using System;
using System.Security.Claims;
using AutoMapper;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class TipoCartaoService : ITipoCartaoService
{
    private readonly ITipoCartaoRepository _tipoCartaoRepository;
    private readonly IMapper _mapper;

    public TipoCartaoService(ITipoCartaoRepository TipoCartaoRepository, IMapper mapper)
    {
        _tipoCartaoRepository = TipoCartaoRepository;
        _mapper = mapper;
    }
    public async Task Add(TipoCartaoDTO TipoCartaoDTO, int userId)
    {
        await GarantirNomeUnico(userId, TipoCartaoDTO.NomeTipoCartao, ignorarId: 0);

        var TipoCartao = _mapper.Map<TipoCartao>(TipoCartaoDTO);
        TipoCartao.UserId = userId;
        await _tipoCartaoRepository.Create(TipoCartao);
    }

    public async Task<IEnumerable<TipoCartaoDTO>> GetTipoCartao(int userId)
    {
        var movimentacoDiaria = await _tipoCartaoRepository.GetTipoCartao(userId);
        return _mapper.Map<IEnumerable<TipoCartaoDTO>>(movimentacoDiaria);
    }

    public async Task<TipoCartaoDTO> GetTipoCartaoById(int id, int userId)
    {
        var TipoCartao = await _tipoCartaoRepository.GetTipoCartaoById(id, userId);
        return _mapper.Map<TipoCartaoDTO>(TipoCartao);
    }

    public async Task Remove(int id, int userId)
    {
        if (await _tipoCartaoRepository.EmUsoAsync(id, userId))
            throw new InvalidOperationException("Não é possível excluir este tipo de cartão, pois ele está em uso por bancos, extratos ou lançamentos.");

        await _tipoCartaoRepository.Remove(id, userId);
    }

    public async Task<bool> UpdateAsync(TipoCartaoDTO dto, int userId)
    {
        await GarantirNomeUnico(userId, dto.NomeTipoCartao, ignorarId: dto.Id);

        var tipoCartao = _mapper.Map<TipoCartao>(dto);
        await _tipoCartaoRepository.UpdateAsync(tipoCartao, userId);
        return true;
    }

    // UX_TipoCartao_UserId_NomeTipoCartao é exata; aqui a regra também ignora maiúsculas e espaços.
    private async Task GarantirNomeUnico(int userId, string? nomeTipoCartao, int ignorarId)
    {
        if (await _tipoCartaoRepository.ExisteNomeAsync(userId, nomeTipoCartao ?? string.Empty, ignorarId))
            throw new InvalidOperationException($"Já existe um tipo de cartão chamado {nomeTipoCartao?.Trim()}.");
    }

}
