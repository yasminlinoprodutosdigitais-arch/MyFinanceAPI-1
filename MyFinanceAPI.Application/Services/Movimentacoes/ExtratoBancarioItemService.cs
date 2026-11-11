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
            await ExigirCategoriasDoUsuarioAsync([dto.CategoriaId], userId);
            var categoriaEscolhida = dto.CategoriaId;
            var tipoEscolhido = dto.TipoMovimentacaoId;

            // Card #79: a pessoa (quem usou o cartão) vem só pelo id; o nome digitado é o do estabelecimento e não liga
            // nem cria pessoa.
            entity.PessoaMovimentacaoId = await PessoaDoUsuarioAsync(dto.PessoaMovimentacaoId ?? dto.PessoaMovimentacao?.Id, userId);

            // Sem escolha no formulário: regra que bate, senão uma regra "igual a" com nome parecido.
            var ativas = await _regraRepository.ListarAtivasAsync(userId);
            var regra = ClassificadorRegras.Encontrar(ativas, entity.Descricao, entity.NomePessoaTransacao, entity.BancoId);
            var parecida = regra == null ? ClassificadorRegras.EncontrarPorSemelhanca(ativas, entity.Descricao, entity.NomePessoaTransacao, entity.BancoId) : null;
            regra ??= parecida;
            entity.CategoriaId = categoriaEscolhida ?? regra?.CategoriaId;
            entity.TipoMovimentacaoId = tipoEscolhido ?? regra?.TipoMovimentacaoId;
            entity.ClassificadoPorSemelhanca = parecida != null && categoriaEscolhida == null && parecida.CategoriaId != null;
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
            // Card #79: o nome que veio no extrato não se edita; no lançamento manual, é o que foi digitado.
            if (existing.ChaveImportacao == null)
                existing.NomePessoaTransacao = dto.NomePessoaTransacao;
            existing.Identificador = dto.Identificador;
            existing.BancoId = dto.BancoId;
            existing.TipoCartaoId = dto.TipoCartaoId;
            existing.TipoMovimentacaoId = dto.TipoMovimentacaoId;
            await ExigirCategoriasDoUsuarioAsync([dto.CategoriaId], userId);
            // Trocar a categoria tira o "por semelhança": passou a ser escolha.
            if (existing.CategoriaId != dto.CategoriaId)
                existing.ClassificadoPorSemelhanca = false;
            existing.CategoriaId = dto.CategoriaId;
            existing.NumeroFatura = dto.NumeroFatura;

            // Card #79: a pessoa vem só pelo id (sem id = sem pessoa) e não mexe no nome do extrato nem na categoria.
            if (dto.AlteraVinculoPessoa)
                existing.PessoaMovimentacaoId = await PessoaDoUsuarioAsync(dto.PessoaMovimentacaoId, userId);

            existing.AbateItemId = dto.AbateItemId;
            await ValidarAbatimentoAsync(existing, userId);

            await _itemRepository.UpdateAsync(existing);
        }

        /// <summary>Card #79: id de pessoa só vale se a pessoa é do usuário; sem id, sem pessoa.</summary>
        private async Task<int?> PessoaDoUsuarioAsync(int? pessoaId, int userId)
        {
            if (pessoaId is not > 0)
                return null;
            return (await _pessoaMovimentacaoRepository.ObterPorIdAsync(pessoaId.Value, userId))?.Id
                ?? throw new RegraNegocioException("Pessoa não encontrada.");
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
            await ExigirCategoriasDoUsuarioAsync(partes.Select(p => p.CategoriaId), userId);

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
        private async Task ExigirCategoriasDoUsuarioAsync(IEnumerable<int?> categorias, int userId)
        {
            if (await _itemRepository.TemCategoriaForaDoUsuarioAsync(categorias.Where(c => c is > 0).Select(c => c!.Value).ToList(), userId))
                throw new RegraNegocioException("Categoria não encontrada.");
        }

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
