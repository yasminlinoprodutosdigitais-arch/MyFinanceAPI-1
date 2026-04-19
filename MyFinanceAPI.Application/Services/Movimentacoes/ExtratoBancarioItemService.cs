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
using MyFinanceAPI.Domain.Interfaces.Repositories;

namespace MyFinanceAPI.Application.Services
{
    public class ExtratoBancarioItemService : IExtratoBancarioItemService
    {
        private readonly IExtratoBancarioItemRepository _itemRepository;
        private readonly IPessoaMovimentacaoRepository _pessoaMovimentacaoRepository;
        private readonly IMapper _mapper;

        public ExtratoBancarioItemService(
            IExtratoBancarioItemRepository itemRepository,
            IPessoaMovimentacaoRepository pessoaMovimentacaoRepository,
            IMapper mapper)
        {
            _itemRepository = itemRepository;
            _pessoaMovimentacaoRepository = pessoaMovimentacaoRepository;
            _mapper = mapper;
        }

        public async Task<ExtratoBancarioItemDTO?> GetByIdAsync(int id, int userId)
        {
            var entity = await _itemRepository.GetByIdAsync(id);
            if (entity == null)
                return null;

            // segurança básica por usuário
            if (entity.UserId != userId)
                throw new UnauthorizedAccessException("Usuário não autorizado para acessar este item.");

            return _mapper.Map<ExtratoBancarioItemDTO>(entity);
        }

        public async Task<IEnumerable<ExtratoBancarioItemDTO>> GetByExtratoAsync(int extratoId, int userId)
        {
            var entities = await _itemRepository.GetByExtratoAsync(extratoId);

            // filtra por UserId, se estiver preenchido
            var filtrados = entities
                .Where(e => e.UserId == null || e.UserId == userId)
                .ToList();

            return _mapper.Map<IEnumerable<ExtratoBancarioItemDTO>>(filtrados);
        }

        public async Task<ExtratoBancarioItemDTO> AddAsync(ExtratoBancarioItemDTO dto, int userId)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var entity = _mapper.Map<ExtratoBancarioItem>(dto);
            entity.PessoaMovimentacao = null;

            entity.UserId = userId;
            
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
            }else
            {
                var pessoaCriada = await _pessoaMovimentacaoRepository.Create(new PessoaMovimentacao
                {
                    NomePessoa = entity.NomePessoaTransacao ?? "Pessoa sem nome",
                    CategoriaId = entity.CategoriaId
                }, userId);
                entity.PessoaMovimentacaoId = pessoaCriada.Id;
                entity.TipoMovimentacaoId = null;
                entity.CategoriaId = null;
            }
            var created = await _itemRepository.CreateAsync(entity);
            return _mapper.Map<ExtratoBancarioItemDTO>(created);
        }

        public async Task UpdateAsync(ExtratoBancarioItemDTO dto, int userId)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var existing = await _itemRepository.GetByIdAsync(dto.Id);
            if (existing == null)
                throw new KeyNotFoundException("Item de extrato não encontrado.");

            if (existing.UserId != userId)
                throw new UnauthorizedAccessException("Usuário não autorizado para alterar este item.");

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
                if (dto.PessoaMovimentacaoId == null || dto.PessoaMovimentacaoId == 0)
                {
                    var pessoaCriada = await _pessoaMovimentacaoRepository.Create(new PessoaMovimentacao
                    {
                        NomePessoa = dto.NomePessoaTransacao ?? "Pessoa sem nome"
                    }, userId);
                    dto.PessoaMovimentacaoId = pessoaCriada.Id;
                    existing.PessoaMovimentacaoId = pessoaCriada.Id;
                }
                var pessoaId = dto.PessoaMovimentacaoId ?? 0;
                var pessoaMovimentacao = await _pessoaMovimentacaoRepository.GetPessoaMovimentacaoById(pessoaId, userId);
                if (pessoaMovimentacao == null)
                {
                    pessoaMovimentacao = new PessoaMovimentacao
                    {
                        NomePessoa = dto.NomePessoaTransacao ?? "Pessoa sem nome"
                    };
                    _pessoaMovimentacaoRepository.Create(pessoaMovimentacao, userId);
                }
                else
                {
                    await _pessoaMovimentacaoRepository.UpdateAsync(new PessoaMovimentacao
                    {
                        Id = pessoaId,
                        NomePessoa = dto.NomePessoaTransacao ?? "Pessoa sem nome",
                        CategoriaId = dto.CategoriaId,
                        TipoMovimentacaoId = dto.TipoMovimentacaoId
                    }, userId);
                }
            }

            await _itemRepository.UpdateAsync(existing);
        }

        public async Task RemoveAsync(int id, int userId)
        {
            var existing = await _itemRepository.GetByIdAsync(id);
            if (existing == null)
                return;

            if (existing.UserId != userId)
                throw new UnauthorizedAccessException("Usuário não autorizado para excluir este item.");

            await _itemRepository.RemoveAsync(id);
        }

        public async Task<IEnumerable<ExtratoBancarioItemDTO>> GetByMonthAsync(
            int userId,
            int year,
            int month, int? bancoId = null, bool ehCredito = false)
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
