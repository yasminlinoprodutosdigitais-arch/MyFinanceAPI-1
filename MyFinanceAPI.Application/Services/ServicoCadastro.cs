using AutoMapper;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

/// <summary>
/// Serviço dos cadastros simples num lugar só (BE-P2-02): mapeia DTO ↔ entidade, grava o dono
/// e chama as validações do cadastro antes de gravar ou excluir. O serviço concreto só
/// sobrescreve os ganchos e o que for realmente dele.
/// </summary>
public abstract class ServicoCadastro<TEntidade, TDto>(IRepositorioDoUsuario<TEntidade> repositorio, IMapper mapper)
    : IServicoCadastro<TDto>
    where TEntidade : class, IEntidade
    where TDto : class
{
    protected IMapper Mapper => mapper;

    /// <summary>Regras de gravação (nome único, tipo do sistema…). <paramref name="ignorarId"/> é 0 ao adicionar.</summary>
    protected virtual Task ValidarGravacaoAsync(TDto dto, int userId, int ignorarId) => Task.CompletedTask;

    /// <summary>Regras de exclusão (em uso, tipo do sistema…).</summary>
    protected virtual Task ValidarExclusaoAsync(int id, int userId) => Task.CompletedTask;

    protected virtual TEntidade ParaEntidade(TDto dto) => mapper.Map<TEntidade>(dto);

    /// <summary>Todo cadastro com BaseEntity tem dono obrigatório; quem não tem, sobrescreve.</summary>
    protected virtual void DefinirDono(TEntidade entidade, int userId)
    {
        if (entidade is BaseEntity doUsuario)
            doUsuario.UserId = userId;
    }

    public virtual async Task<IEnumerable<TDto>> ListarAsync(int userId) =>
        mapper.Map<IEnumerable<TDto>>(await repositorio.ListarAsync(userId));

    public virtual async Task<TDto?> ObterPorIdAsync(int id, int userId)
    {
        var entidade = await repositorio.ObterPorIdAsync(id, userId);
        return entidade == null ? null : mapper.Map<TDto>(entidade);
    }

    public virtual async Task<TDto> AdicionarAsync(TDto dto, int userId)
    {
        await ValidarGravacaoAsync(dto, userId, ignorarId: 0);

        var entidade = ParaEntidade(dto);
        DefinirDono(entidade, userId);
        return mapper.Map<TDto>(await repositorio.CriarAsync(entidade));
    }

    public virtual async Task AtualizarAsync(TDto dto, int userId)
    {
        var entidade = ParaEntidade(dto);
        await ValidarGravacaoAsync(dto, userId, ignorarId: entidade.Id);

        await repositorio.AtualizarAsync(entidade, userId);
    }

    public virtual async Task RemoverAsync(int id, int userId)
    {
        await ValidarExclusaoAsync(id, userId);
        await repositorio.RemoverAsync(id, userId);
    }
}
