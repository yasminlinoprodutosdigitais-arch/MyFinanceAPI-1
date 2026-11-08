// MyFinanceAPI.Application/Services/ExtratoBancarioItemService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MyFinanceAPI.Application.DTO.Extrato;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services
{
    public class ExtratoBancarioItemService : IExtratoBancarioItemService
    {
        private readonly IExtratoBancarioItemRepository _itemRepository;
        private readonly IPessoaMovimentacaoRepository _pessoaMovimentacaoRepository;
        private readonly IRegraClassificacaoRepository _regraRepository;
        private readonly IMapper _mapper;

        public ExtratoBancarioItemService(
            IExtratoBancarioItemRepository itemRepository,
            IPessoaMovimentacaoRepository pessoaMovimentacaoRepository,
            IRegraClassificacaoRepository regraRepository,
            IMapper mapper)
        {
            _itemRepository = itemRepository;
            _pessoaMovimentacaoRepository = pessoaMovimentacaoRepository;
            _regraRepository = regraRepository;
            _mapper = mapper;
        }

        public async Task<ExtratoBancarioItemDTO?> GetByIdAsync(int id, int userId)
        {
            var entity = await _itemRepository.GetByIdAsync(id);
            if (entity == null)
                return null;

            // segurança básica por usuário
            if (entity.UsuarioId != userId)
                throw new UnauthorizedAccessException("Usuário não autorizado para acessar este item.");

            return _mapper.Map<ExtratoBancarioItemDTO>(entity);
        }

        public async Task<ExtratoBancarioItemDTO> AddAsync(ExtratoBancarioItemDTO dto, int userId)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            NormalizarIds(dto);
            var entity = _mapper.Map<ExtratoBancarioItem>(dto);
            entity.PessoaMovimentacao = null;
            // Item manual nunca tem chave de importação (ela é a garantia de idempotência do import).
            entity.ChaveImportacao = null;

            entity.UsuarioId = userId;

            // Escolhidos no formulário: prevalecem sobre o vínculo da pessoa; sem escolha, o item herda
            // o vínculo (decisão de 14/10/2026 — antes a escolha era descartada quando havia pessoa).
            var categoriaEscolhida = dto.CategoriaId;
            var tipoEscolhido = dto.TipoMovimentacaoId;

            var pessoaCadastrada = await _pessoaMovimentacaoRepository.VerificaPossuiPessoa(entity.NomePessoaTransacao, userId);
            if(dto.PessoaMovimentacao != null)
            {
                entity.PessoaMovimentacaoId = dto.PessoaMovimentacao.Id;
                entity.NomePessoaTransacao = dto.PessoaMovimentacao.NomePessoa;
                entity.TipoMovimentacaoId = dto.PessoaMovimentacao.TipoMovimentacaoId;
                entity.CategoriaId = dto.PessoaMovimentacao.CategoriaId;
            } else if (pessoaCadastrada != null && pessoaCadastrada.Count() > 0)            {
                var pessoaId = pessoaCadastrada.First().Id;
                var tipoMovimentacaoId = pessoaCadastrada.First().TipoMovimentacaoId ?? 0;
                var categoriaId = pessoaCadastrada.First().CategoriaId ?? 0;

                entity.PessoaMovimentacaoId = pessoaId;
                entity.TipoMovimentacaoId = tipoMovimentacaoId != 0 ? tipoMovimentacaoId : null;
                entity.CategoriaId = categoriaId != 0 ? categoriaId : null;
            }
            else
            {
                // Sem nome, ou nome que não é pessoa cadastrada (card #76, fase 2): o nome fica só no lançamento.
                entity.PessoaMovimentacaoId = null;
            }

            // Card #76: sem escolha no formulário, a regra vem antes do padrão da pessoa.
            var regra = ClassificadorRegras.Encontrar(await _regraRepository.ListarAtivasAsync(userId), entity.Descricao, entity.NomePessoaTransacao, entity.BancoId);
            entity.CategoriaId = categoriaEscolhida ?? regra?.CategoriaId ?? entity.CategoriaId;
            entity.TipoMovimentacaoId = tipoEscolhido ?? regra?.TipoMovimentacaoId ?? entity.TipoMovimentacaoId;
            entity.AbateItemId = dto.AbateItemId;
            await ValidarAbatimentoAsync(entity, userId);

            var created = await _itemRepository.CreateAsync(entity);
            // A regra conta como usada quando decidiu a categoria ou o tipo (nada escolhido no formulário).
            if (regra != null && ((categoriaEscolhida == null && regra.CategoriaId != null) || (tipoEscolhido == null && regra.TipoMovimentacaoId != null)))
                await _regraRepository.RegistrarUsoAsync(userId, new Dictionary<int, int> { [regra.Id] = 1 });
            return _mapper.Map<ExtratoBancarioItemDTO>(created);
        }

        /// <summary>
        /// Referência "vazia" vinda como 0 (padrão de algumas telas) vira null: Id 0 não existe e a FK
        /// recusaria a gravação com erro 500.
        /// </summary>
        private static void NormalizarIds(ExtratoBancarioItemDTO dto)
        {
            static int? IdOuNulo(int? id) => id is > 0 ? id : null;

            dto.CategoriaId = IdOuNulo(dto.CategoriaId);
            dto.TipoMovimentacaoId = IdOuNulo(dto.TipoMovimentacaoId);
            dto.TipoCartaoId = IdOuNulo(dto.TipoCartaoId);
            dto.BancoId = IdOuNulo(dto.BancoId);
            dto.PessoaMovimentacaoId = IdOuNulo(dto.PessoaMovimentacaoId);
        }

        public async Task UpdateAsync(ExtratoBancarioItemDTO dto, int userId)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            NormalizarIds(dto);
            var existing = await _itemRepository.GetByIdAsync(dto.Id);
            if (existing == null)
                throw new KeyNotFoundException("Item de extrato não encontrado.");

            if (existing.UsuarioId != userId)
                throw new UnauthorizedAccessException("Usuário não autorizado para alterar este item.");

            if (existing.Dividido && existing.Valor != dto.Valor)
                throw new RegraNegocioException("Este lançamento está dividido: desfaça a divisão para mudar o valor.");

            // Atualiza campos permitidos
            existing.DataMovimentacao = dto.DataMovimentacao;
            existing.Valor = dto.Valor;
            existing.TipoLancamento = dto.TipoLancamento;
            existing.Descricao = dto.Descricao;
            existing.Observacao = dto.Observacao;
            existing.NomePessoaTransacao = dto.NomePessoaTransacao;
            existing.Identificador = dto.Identificador;
            existing.BancoId = dto.BancoId;
            existing.TipoCartaoId = dto.TipoCartaoId;
            existing.TipoMovimentacaoId = dto.TipoMovimentacaoId;
            existing.CategoriaId = dto.CategoriaId;
            existing.NumeroFatura = dto.NumeroFatura;

            if (dto.AlteraVinculoPessoa)
            {
                if (string.IsNullOrWhiteSpace(dto.NomePessoaTransacao))
                {
                    // Sem nome de pessoa → item sem pessoa.
                    existing.PessoaMovimentacaoId = null;
                }
                else
                {
                    var pessoaId = dto.PessoaMovimentacaoId ?? 0;
                    var pessoaMovimentacao = pessoaId == 0
                        ? null
                        : await _pessoaMovimentacaoRepository.ObterPorIdAsync(pessoaId, userId);

                    // Card #76, fase 2: sem id, liga pelo nome a uma pessoa já cadastrada; nome novo fica só no lançamento.
                    pessoaMovimentacao ??= (await _pessoaMovimentacaoRepository.VerificaPossuiPessoa(dto.NomePessoaTransacao, userId))
                        .FirstOrDefault();

                    // Card #45, parte 2: trocar a pessoa de um lançamento não reescreve mais a categoria/tipo
                    // padrão dela (antes a edição sobrescrevia sem avisar). O padrão muda em Configurar pessoas.
                    existing.PessoaMovimentacaoId = pessoaMovimentacao?.Id;
                }
            }

            existing.AbateItemId = dto.AbateItemId;
            await ValidarAbatimentoAsync(existing, userId);

            await _itemRepository.UpdateAsync(existing);
        }

        /// <summary>
        /// Card #45, parte 3: só um pagamento (Entrada) abate uma compra, e a compra tem que ser uma Saída da mesma pessoa,
        /// do mesmo usuário.
        /// </summary>
        private async Task ValidarAbatimentoAsync(ExtratoBancarioItem pagamento, int userId)
        {
            if (pagamento.AbateItemId is not > 0)
            {
                pagamento.AbateItemId = null;
                return;
            }

            if (pagamento.TipoLancamento != TipoLancamentoExtrato.Entrada)
                throw new RegraNegocioException("Só um pagamento recebido (entrada) pode abater uma compra.");

            var compra = await _itemRepository.GetByIdAsync(pagamento.AbateItemId.Value);
            // Dividida (card #76, fase 3): vale a pessoa de alguma parte.
            var mesmaPessoa = compra != null && pagamento.PessoaMovimentacaoId != null && (compra.Dividido
                ? compra.Partes.Any(p => p.PessoaMovimentacaoId == pagamento.PessoaMovimentacaoId)
                : compra.PessoaMovimentacaoId == pagamento.PessoaMovimentacaoId);
            if (compra == null || compra.UsuarioId != userId || compra.TipoLancamento != TipoLancamentoExtrato.Saida || !mesmaPessoa)
                throw new RegraNegocioException("A compra abatida tem que ser uma saída da mesma pessoa.");
        }

        public async Task DividirAsync(int id, IReadOnlyList<ExtratoItemParteDTO> partes, int userId)
        {
            var item = await _itemRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Item de extrato não encontrado.");
            if (item.UsuarioId != userId)
                throw new UnauthorizedAccessException("Usuário não autorizado para alterar este item.");

            ValidarPartes(item, partes);

            static int? IdOuNulo(int? v) => v is > 0 ? v : null;
            var pessoas = partes.Select(p => IdOuNulo(p.PessoaMovimentacaoId)).OfType<int>().Distinct().ToList();
            foreach (var pessoaId in pessoas)
            {
                if (await _pessoaMovimentacaoRepository.ObterPorIdAsync(pessoaId, userId) == null)
                    throw new RegraNegocioException("Pessoa da parte não encontrada.");
            }

            // Um pagamento que já abate esta compra precisa continuar sendo de quem deve alguma parte dela.
            var podemAbater = partes.Count > 0 ? pessoas.Select(p => (int?)p).ToHashSet() : new HashSet<int?> { item.PessoaMovimentacaoId };
            if ((await _itemRepository.PessoasQueAbatemAsync(id)).Any(p => !podemAbater.Contains(p)))
                throw new RegraNegocioException("Há pagamento abatendo esta compra de uma pessoa que ficaria fora da divisão.");

            await _itemRepository.SubstituirPartesAsync(id, partes.Select((p, i) => new ExtratoItemParte
            {
                ExtratoBancarioItemId = id,
                UsuarioId = userId,
                Valor = p.Valor,
                CategoriaId = IdOuNulo(p.CategoriaId),
                TipoMovimentacaoId = IdOuNulo(p.TipoMovimentacaoId),
                PessoaMovimentacaoId = IdOuNulo(p.PessoaMovimentacaoId),
                Descricao = string.IsNullOrWhiteSpace(p.Descricao) ? null : p.Descricao.Trim(),
                Ordem = i
            }).ToList());
        }

        /// <summary>Só saída se divide; 2 ou mais partes positivas cuja soma é o valor do lançamento (vazio = desfazer).</summary>
        public static void ValidarPartes(ExtratoBancarioItem item, IReadOnlyList<ExtratoItemParteDTO> partes)
        {
            if (partes.Count == 0)
                return;
            if (item.TipoLancamento != TipoLancamentoExtrato.Saida)
                throw new RegraNegocioException("Só uma saída pode ser dividida.");
            if (partes.Count < 2)
                throw new RegraNegocioException("Divida em pelo menos 2 partes.");
            if (partes.Any(p => p.Valor <= 0))
                throw new RegraNegocioException("Cada parte precisa ter um valor maior que zero.");
            if (partes.Sum(p => p.Valor) != item.Valor)
                throw new RegraNegocioException("A soma das partes tem que ser igual ao valor do lançamento.");
        }

        public async Task RemoveAsync(int id, int userId)
        {
            var existing = await _itemRepository.GetByIdAsync(id);
            if (existing == null)
                return;

            if (existing.UsuarioId != userId)
                throw new UnauthorizedAccessException("Usuário não autorizado para excluir este item.");

            await _itemRepository.RemoveAsync(id);
        }

        public async Task<IEnumerable<ExtratoBancarioItemDTO>> GetByMonthAsync(
            int userId,
            int year,
            int month, int? bancoId = null)
        {
            var inicio = new DateOnly(year, month, 1);
            var fimExclusive = inicio.AddMonths(1);
            var numeroFatura = year + "-" + month.ToString().PadLeft(2, '0');

            var entities =  await _itemRepository.GetByUserAndMonthAsync(
                    userId,
                    inicio,
                    fimExclusive, numeroFatura,bancoId);

            return _mapper.Map<IEnumerable<ExtratoBancarioItemDTO>>(entities);
        }


    }
}
