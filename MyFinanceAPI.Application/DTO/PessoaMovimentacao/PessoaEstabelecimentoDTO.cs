namespace MyFinanceAPI.Application.DTO;

/// <summary>
/// Pessoa que parece estabelecimento (card #76, fase 2): criada pela importação antiga, fora dos acertos, sem padrão e
/// sem regra. Candidata à limpeza em "Configurar pessoas".
/// </summary>
public class PessoaEstabelecimentoDTO
{
    public int Id { get; set; }
    public string NomePessoa { get; set; } = "";
    /// <summary>Quantos lançamentos estão ligados a ela (continuam com o nome depois da limpeza).</summary>
    public int Lancamentos { get; set; }
}

/// <summary>Resultado da limpeza: quantas pessoas foram apagadas.</summary>
public class PessoasRemovidasDTO
{
    public int Quantidade { get; set; }
}
