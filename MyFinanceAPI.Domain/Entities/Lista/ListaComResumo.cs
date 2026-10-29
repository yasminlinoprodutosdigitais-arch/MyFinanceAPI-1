namespace MyFinanceAPI.Domain.Entities;

/// <summary>
/// Lista com os números do card no Cadastro (card #67), calculados numa consulta só.
/// Total soma quantidade × valor no modo UNITARIO (sem quantidade conta 1) e o valor no modo TOTAL;
/// item sem valor fica fora. ProximaData é a menor data de hoje em diante entre os pendentes.
/// </summary>
public class ListaComResumo
{
    public Lista Lista { get; init; } = null!;
    public int Itens { get; init; }
    public int Feitos { get; init; }
    public decimal Total { get; init; }
    public decimal TotalFeito { get; init; }
    public DateOnly? ProximaData { get; init; }
}
