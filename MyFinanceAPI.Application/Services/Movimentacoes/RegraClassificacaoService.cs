using AutoMapper;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

/// <summary>Cadastro das regras de classificação (card #76, fase 1).</summary>
public class RegraClassificacaoService(IRegraClassificacaoRepository regras, IMapper mapper)
    : ServicoCadastro<RegraClassificacao, RegraClassificacaoDTO>(regras, mapper), IRegraClassificacaoService
{
    protected override RegraClassificacao ParaEntidade(RegraClassificacaoDTO dto)
    {
        var regra = base.ParaEntidade(dto);
        regra.Texto = RegraClassificacao.Normalizar(dto.Texto) ?? "";
        regra.Campo = CampoOuPadrao(dto.Campo);
        regra.Operador = OperadorOuPadrao(dto.Operador);
        regra.BancoId = dto.BancoId is > 0 ? dto.BancoId : null;
        regra.CategoriaId = dto.CategoriaId is > 0 ? dto.CategoriaId : null;
        regra.TipoMovimentacaoId = dto.TipoMovimentacaoId is > 0 ? dto.TipoMovimentacaoId : null;
        regra.PessoaMovimentacaoId = dto.PessoaMovimentacaoId is > 0 ? dto.PessoaMovimentacaoId : null;
        return regra;
    }

    protected override async Task ValidarGravacaoAsync(RegraClassificacaoDTO dto, int userId, int ignorarId)
    {
        var texto = RegraClassificacao.Normalizar(dto.Texto)
            ?? throw new RegraNegocioException("O texto da regra é obrigatório.");
        if (texto.Length < 2)
            throw new RegraNegocioException("O texto da regra precisa ter pelo menos 2 letras.");
        if (!string.IsNullOrWhiteSpace(dto.Campo) && !CamposRegra.Todos.Contains(dto.Campo))
            throw new RegraNegocioException("Escolha onde procurar o texto: a descrição, a pessoa ou as duas.");
        if (!string.IsNullOrWhiteSpace(dto.Operador) && !OperadoresRegra.Todos.Contains(dto.Operador))
            throw new RegraNegocioException("Escolha como comparar o texto: contém, começa com ou é igual.");
        if (dto.CategoriaId is not > 0 && dto.TipoMovimentacaoId is not > 0 && dto.PessoaMovimentacaoId is not > 0)
            throw new RegraNegocioException("Escolha uma categoria, um tipo ou uma pessoa para a regra.");
        var bancoId = dto.BancoId is > 0 ? dto.BancoId : null;
        if (await regras.ExisteFraseAsync(userId, CampoOuPadrao(dto.Campo), OperadorOuPadrao(dto.Operador), texto, bancoId, ignorarId))
            throw new RegraNegocioException($"Já existe uma regra igual para \"{texto}\".");
    }

    public Task ReordenarAsync(int userId, IReadOnlyList<int> ids) => regras.ReordenarAsync(userId, ids);

    internal static string CampoOuPadrao(string? campo) =>
        string.IsNullOrWhiteSpace(campo) ? CamposRegra.DescricaoOuPessoa : campo;

    internal static string OperadorOuPadrao(string? operador) =>
        string.IsNullOrWhiteSpace(operador) ? OperadoresRegra.Contem : operador;
}

/// <summary>Qual regra vale para um lançamento (puro, para testar).</summary>
public static class ClassificadorRegras
{
    /// <summary>
    /// A primeira regra ligada, na ordem de prioridade (Ordem, depois Id), que bate com o lançamento. A comparação é sem
    /// acento e em maiúsculas.
    /// </summary>
    public static RegraClassificacao? Encontrar(IEnumerable<RegraClassificacao> regras, string? descricao, string? nomePessoa, int? bancoId)
    {
        var desc = RegraClassificacao.Normalizar(descricao);
        var pessoa = RegraClassificacao.Normalizar(nomePessoa);
        if (desc == null && pessoa == null)
            return null;

        return regras
            .OrderBy(r => r.Ordem).ThenBy(r => r.Id)
            .FirstOrDefault(r => Bate(r, desc, pessoa, bancoId));
    }

    /// <summary>A regra (ligada) pega este lançamento? Descrição e pessoa já normalizadas.</summary>
    public static bool Bate(RegraClassificacao regra, string? descricaoNormalizada, string? pessoaNormalizada, int? bancoId)
    {
        if (!regra.Ativa || string.IsNullOrEmpty(regra.Texto))
            return false;
        if (regra.BancoId != null && regra.BancoId != bancoId)
            return false;

        IEnumerable<string?> alvos = regra.Campo switch
        {
            CamposRegra.Descricao => [descricaoNormalizada],
            CamposRegra.Pessoa => [pessoaNormalizada],
            CamposRegra.NomeExtrato => [NomeExtrato.Chave(descricaoNormalizada, pessoaNormalizada)],
            _ => [descricaoNormalizada, pessoaNormalizada]
        };
        return alvos.OfType<string>().Where(a => a.Length > 0).Any(a => regra.Operador switch
        {
            OperadoresRegra.ComecaCom => a.StartsWith(regra.Texto, StringComparison.Ordinal),
            OperadoresRegra.Igual => a == regra.Texto,
            _ => a.Contains(regra.Texto, StringComparison.Ordinal)
        });
    }

    /// <summary>
    /// Card #79: sem regra que bata, a primeira regra ligada "é igual a" (nome do extrato ou pessoa) cujo texto é
    /// <see cref="NomeExtrato.Parecidos"/> com o nome do extrato deste lançamento. Aplica sozinha e marca "por semelhança".
    /// </summary>
    public static RegraClassificacao? EncontrarPorSemelhanca(IEnumerable<RegraClassificacao> regras, string? descricao, string? nomePessoa, int? bancoId)
    {
        var chave = NomeExtrato.Chave(descricao, nomePessoa);
        if (chave.Length == 0)
            return null;

        return regras
            .Where(r => r.Ativa && r.Operador == OperadoresRegra.Igual && !string.IsNullOrEmpty(r.Texto)
                && r.Campo is CamposRegra.NomeExtrato or CamposRegra.Pessoa
                && (r.BancoId == null || r.BancoId == bancoId))
            .OrderBy(r => r.Ordem).ThenBy(r => r.Id)
            .FirstOrDefault(r => NomeExtrato.Parecidos(r.Texto, chave));
    }
}
