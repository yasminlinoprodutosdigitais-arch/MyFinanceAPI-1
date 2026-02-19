using System;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces.Repositories;

namespace MyFinanceAPI.Application.Services;

public class VinculoTipoMovimentacaoService : IVinculoTipoMovimentacaoService
{
    private readonly IVinculoTipoMovimentacaoRepository _repo;
    private readonly IExtratoBancarioItemRepository _extratoBancarioItemRepository;
    public VinculoTipoMovimentacaoService(
        IVinculoTipoMovimentacaoRepository repo)
    {
        _repo = repo;
        
    }

    public async Task<VinculoTipoMovimentacao> CriarSeNaoExistirAsync(
        int userId, string chave, string descricaoOriginal)
    {
        var existe = await _repo.GetByChaveAsync(userId, chave);

        if (existe != null)
            return existe;

        var novo = new VinculoTipoMovimentacao
        {
            UserId = userId,
            ChaveDescricao = chave,
            DescricaoOriginalExemplo = descricaoOriginal
        };

        await _repo.AddAsync(novo);
        return novo;
    }

    public async Task<List<VinculoTipoMovimentacao>> ObterPendentesAsync(int userId)
    {
        return await _repo.GetPendentesAsync(userId);
    }

    public async Task AtualizarVinculoAsync(int id, VinculoUpdateDTO dto, int userId)
    {
        var vinculo = await _repo.GetByIdAsync(id, userId);

        if (vinculo == null)
            throw new Exception("Vínculo não encontrado.");

        vinculo.TipoMovimentacaoId = dto.TipoMovimentacaoId;
        vinculo.CategoriaId = dto.CategoriaId;
        vinculo.DataUltimoUso = DateTime.Now;

        await _repo.UpdateAsync(vinculo);
    }

}
