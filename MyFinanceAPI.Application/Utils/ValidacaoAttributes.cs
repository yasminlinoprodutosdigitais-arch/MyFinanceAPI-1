using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceAPI.Application.Utils;

// BE-P4-17: validação de entrada com mensagem em português. Cada regra espelha uma CHECK ou um NOT NULL
// do banco: o [ApiController] recusa com 400 antes de chegar lá, e o banco fica como rede de segurança.

/// <summary>Campo obrigatório (texto vazio também é recusado).</summary>
public sealed class ObrigatorioAttribute : RequiredAttribute
{
    public ObrigatorioAttribute()
    {
        ErrorMessage = "{0} é obrigatório.";
    }
}

/// <summary>Só os valores do domínio (as CHECKs <c>IN (...)</c>). Nulo passa: quem exige é o <see cref="ObrigatorioAttribute"/>.</summary>
public sealed class UmDeAttribute : ValidationAttribute
{
    private readonly object[] _valores;

    public UmDeAttribute(params object[] valores)
    {
        _valores = valores;
        ErrorMessage = "{0} aceita só: " + string.Join(", ", valores) + ".";
    }

    public override bool IsValid(object? value) => value is null || _valores.Contains(value);
}

/// <summary>Mínimo numérico (as CHECKs <c>coluna &gt;= n</c>). Nulo passa.</summary>
public sealed class MinimoAttribute(double minimo) : ValidationAttribute("{0} não pode ser menor que " + minimo + ".")
{
    public override bool IsValid(object? value) => value is null || Convert.ToDouble(value) >= minimo;
}

/// <summary>Faixa numérica (as CHECKs <c>BETWEEN</c>).</summary>
public sealed class FaixaAttribute : RangeAttribute
{
    public FaixaAttribute(int minimo, int maximo) : base(minimo, maximo)
    {
        ErrorMessage = "{0} deve estar entre {1} e {2}.";
    }
}

/// <summary>Lista de dias de vencimento: cada um de 1 a 31 (contas_vencimento_dia_check). Nula passa.</summary>
public sealed class DiasDoMesAttribute() : ValidationAttribute("{0}: cada dia deve estar entre 1 e 31.")
{
    public override bool IsValid(object? value) =>
        value is not IEnumerable dias || dias.Cast<object>().All(d => d is int dia && dia is >= 1 and <= 31);
}
