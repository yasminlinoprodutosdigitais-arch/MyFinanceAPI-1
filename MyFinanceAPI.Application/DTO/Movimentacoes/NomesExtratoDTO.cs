using MyFinanceAPI.Application.Services;

namespace MyFinanceAPI.Application.DTO;

/// <summary>Um nome que veio nos extratos ("CIRCUITO") e como ele está classificado.</summary>
public class NomeExtratoDTO
{
    public string Texto { get; set; } = "";
    public int Lancamentos { get; set; }
    public DateOnly UltimoUso { get; set; }
    /// <summary>Lançamentos desse nome ainda sem categoria.</summary>
    public int SemCategoria { get; set; }
    /// <summary><see cref="CoberturaNome"/>.</summary>
    public string Cobertura { get; set; } = CoberturaNome.Nenhuma;
    /// <summary>A regra que classifica (ou a parecida, na semelhança).</summary>
    public int? RegraId { get; set; }
    public string? RegraTexto { get; set; }
    public int? CategoriaId { get; set; }
    public int? TipoMovimentacaoId { get; set; }
    /// <summary>Pelo histórico desse nome (ou pela regra parecida); só quando não há regra.</summary>
    public int? SugestaoCategoriaId { get; set; }
    public int? SugestaoTipoMovimentacaoId { get; set; }
}

public class ClassificarNomeDTO
{
    public string Texto { get; set; } = "";
    public int? CategoriaId { get; set; }
    public int? TipoMovimentacaoId { get; set; }
}

public class ClassificarNomesDTO
{
    public List<ClassificarNomeDTO> Itens { get; set; } = [];
    /// <summary>Também nos lançamentos já importados desse nome sem categoria ou classificados por semelhança.</summary>
    public bool AplicarAosImportados { get; set; } = true;
}

public class ClassificacaoNomesResultadoDTO
{
    /// <summary>Regras criadas ou alteradas.</summary>
    public int Regras { get; set; }
    /// <summary>Lançamentos atualizados.</summary>
    public int Lancamentos { get; set; }
}

/// <summary>Regras "igual a" com nomes parecidos (o mesmo lugar escrito de jeitos diferentes).</summary>
public class GrupoParecidosDTO
{
    public List<RegraParecidaDTO> Regras { get; set; } = [];
    /// <summary>As regras do grupo classificam diferente: é preciso escolher a que vale.</summary>
    public bool Divergem { get; set; }
}

public class RegraParecidaDTO
{
    public int Id { get; set; }
    public string Texto { get; set; } = "";
    public int? CategoriaId { get; set; }
    public int? TipoMovimentacaoId { get; set; }
    public int Lancamentos { get; set; }
}

public class JuntarNomesDTO
{
    public List<int> RegraIds { get; set; } = [];
    public int ManterRegraId { get; set; }
    public bool AplicarAosImportados { get; set; } = true;
}
