using System.ComponentModel.DataAnnotations;

namespace MyFinanceAPI.Application.Utils;

/// <summary>
/// DB-P3-04: mesmo limite da coluna (<c>HasMaxLength</c>), recusado pelo [ApiController] com 400
/// e mensagem em português antes de chegar ao banco.
/// </summary>
public sealed class TextoAteAttribute : MaxLengthAttribute
{
    public TextoAteAttribute(int limite) : base(limite)
    {
        ErrorMessage = "{0} aceita no máximo {1} caracteres.";
    }
}
