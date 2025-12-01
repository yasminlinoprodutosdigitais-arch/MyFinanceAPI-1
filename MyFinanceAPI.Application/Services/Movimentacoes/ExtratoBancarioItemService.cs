// MyFinanceAPI.Application/Services/ExtratoBancarioItemService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MyFinanceAPI.Application.DTO.Extrato;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces.Repositories;

namespace MyFinanceAPI.Application.Services
{
    public class ExtratoBancarioItemService : IExtratoBancarioItemService
    {
        private readonly IExtratoBancarioItemRepository _itemRepository;
        private readonly IMapper _mapper;

        public ExtratoBancarioItemService(
            IExtratoBancarioItemRepository itemRepository,
            IMapper mapper)
        {
            _itemRepository = itemRepository;
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
            entity.UserId = userId;

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
            existing.NomePessoaTransacao = dto.NomePessoaTransacao;
            existing.Identificador = dto.Identificador;
            existing.BancoId = dto.BancoId;
            existing.TipoCartaoId = dto.TipoCartaoId;
            existing.TipoMovimentacaoId = dto.TipoMovimentacaoId;

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
            int month)
        {
            var inicio = new DateOnly(year, month, 1);
            var fimExclusive = inicio.AddMonths(1);

            var entities = await _itemRepository.GetByUserAndMonthAsync(
                userId,
                inicio,
                fimExclusive);

            return _mapper.Map<IEnumerable<ExtratoBancarioItemDTO>>(entities);
        }

        
    }
}
