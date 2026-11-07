using MyFinanceAPI.Application.Utils;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.DTO;

/// <summary>
/// Regra de classificação (card #76, fase 1) como frase: "Se Campo Operador Texto em Banco (ou todas as contas), então
/// categoria, tipo e pessoa". O texto volta normalizado (sem acento, maiúsculas). Ordem, uso e última aplicação só se
/// leem: mudam por reordenar e quando a regra classifica.
/// </summary>
public class RegraClassificacaoDTO
{
    public int Id { get; set; }
    [TextoAte(120)] public string Texto { get; set; } = "";
    /// <summary>descricao_ou_pessoa (padrão), descricao ou pessoa.</summary>
    [TextoAte(20)] public string Campo { get; set; } = CamposRegra.DescricaoOuPessoa;
    /// <summary>contem (padrão), comeca_com ou igual.</summary>
    [TextoAte(20)] public string Operador { get; set; } = OperadoresRegra.Contem;
    /// <summary>Só lançamentos desta conta; null = todas.</summary>
    public int? BancoId { get; set; }
    public string? BancoNome { get; set; }
    public int? CategoriaId { get; set; }
    public string? CategoriaNome { get; set; }
    public int? TipoMovimentacaoId { get; set; }
    public string? TipoMovimentacaoNome { get; set; }
    public int? PessoaMovimentacaoId { get; set; }
    public string? PessoaNome { get; set; }
    public bool Ativa { get; set; } = true;
    public int Ordem { get; set; }
    public int VezesAplicada { get; set; }
    public DateTime? UltimaAplicacao { get; set; }
}

/// <summary>A frase de uma regra ainda não gravada, para a prévia ao vivo.</summary>
public class PreviaRegraDTO
{
    [TextoAte(120)] public string? Texto { get; set; }
    [TextoAte(20)] public string? Campo { get; set; }
    [TextoAte(20)] public string? Operador { get; set; }
    public int? BancoId { get; set; }
    /// <summary>A categoria que a regra vai dar: lançamentos já em outra categoria contam como conflito.</summary>
    public int? CategoriaId { get; set; }
}

/// <summary>O que a regra pegaria nos lançamentos que já existem.</summary>
public class PreviaRegraResultadoDTO
{
    /// <summary>Todos os lançamentos que batem.</summary>
    public int Total { get; set; }
    /// <summary>Os que estão sem categoria (o "aplicar" só mexe nesses).</summary>
    public int SemCategoria { get; set; }
    /// <summary>Os que já estão em outra categoria (a regra não muda eles).</summary>
    public int Conflitos { get; set; }
    /// <summary>Até 5 exemplos, os sem categoria primeiro, depois os mais recentes.</summary>
    public List<ExemploRegraDTO> Exemplos { get; set; } = [];
    /// <summary>Até 3 que já estão em outra categoria, para o aviso de conflito.</summary>
    public List<ExemploRegraDTO> ExemplosConflito { get; set; } = [];
}

public class ExemploRegraDTO
{
    public int Id { get; set; }
    public DateOnly DataMovimentacao { get; set; }
    /// <summary>A descrição, ou o nome da pessoa quando não há descrição.</summary>
    public string Texto { get; set; } = "";
    public decimal Valor { get; set; }
    public string? TipoLancamento { get; set; }
    public int? CategoriaId { get; set; }
    public string? CategoriaNome { get; set; }
}

/// <summary>Resultado do "aplicar": quantos e quais lançamentos ganharam categoria.</summary>
public class AplicacaoRegraDTO
{
    public int Quantidade { get; set; }
    public List<int> ItemIds { get; set; } = [];
}

/// <summary>Texto que se repete em lançamentos sem categoria e ainda não tem regra ("Sugestões do seu extrato").</summary>
public class SugestaoRegraDTO
{
    public string Texto { get; set; } = "";
    /// <summary>Em quantos lançamentos sem categoria o texto aparece.</summary>
    public int Quantidade { get; set; }
    /// <summary>A categoria mais usada nesse texto, se você já classificou algum.</summary>
    public int? CategoriaId { get; set; }
    public string? CategoriaNome { get; set; }
    /// <summary>Quantas vezes essa categoria foi usada.</summary>
    public int VezesCategoria { get; set; }
}
