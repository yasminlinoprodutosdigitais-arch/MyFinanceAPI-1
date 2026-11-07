namespace MyFinanceAPI.Application.DTO;

/// <summary>Sugestão de categoria de um lançamento sem categoria, pelo histórico (card #76, fase 1).</summary>
public class SugestaoCategoriaDTO
{
    public int ItemId { get; set; }
    public int CategoriaId { get; set; }
    /// <summary>Quantas vezes essa categoria foi usada na mesma descrição.</summary>
    public int Vezes { get; set; }
}
