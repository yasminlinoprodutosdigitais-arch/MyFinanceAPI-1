using AutoMapper;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class TipoCartaoService(ITipoCartaoRepository tipoCartaoRepository, IMapper mapper)
    : ServicoCadastro<TipoCartao, TipoCartaoDTO>(tipoCartaoRepository, mapper), ITipoCartaoService
{
    // TipoCartao não herda BaseEntity: o dono é opcional (padrão do sistema).
    protected override void DefinirDono(TipoCartao entidade, int userId) => entidade.UsuarioId = userId;

    protected override async Task ValidarGravacaoAsync(TipoCartaoDTO dto, int userId, int ignorarId)
    {
        if (ignorarId != 0)
            await GarantirQueNaoEhDoSistema(ignorarId, userId);

        // UX_TipoCartao_UserId_NomeTipoCartao é exata; aqui a regra também ignora maiúsculas e espaços.
        if (await tipoCartaoRepository.ExisteNomeAsync(userId, dto.NomeTipoCartao ?? string.Empty, ignorarId))
            throw new RegraNegocioException($"Já existe um tipo de cartão chamado {dto.NomeTipoCartao?.Trim()}.");
    }

    protected override async Task ValidarExclusaoAsync(int id, int userId)
    {
        await GarantirQueNaoEhDoSistema(id, userId);

        if (await tipoCartaoRepository.EmUsoAsync(id, userId))
            throw new RegraNegocioException("Não é possível excluir este tipo de cartão, pois ele está em uso por bancos, extratos ou lançamentos.");
    }

    // O repositório já não altera nem remove tipos do sistema; isto só dá a mensagem certa.
    private async Task GarantirQueNaoEhDoSistema(int id, int userId)
    {
        var tipo = await tipoCartaoRepository.ObterPorIdAsync(id, userId);
        if (tipo?.EhSistema == true)
            throw new RegraNegocioException("Os tipos padrão do sistema não podem ser alterados nem excluídos.");
    }
}
