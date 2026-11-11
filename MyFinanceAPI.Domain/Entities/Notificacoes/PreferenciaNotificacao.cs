namespace MyFinanceAPI.Domain.Entities;

/// <summary>
/// Preferências de notificação do usuário (card #73): uma linha por usuário. <see cref="Config"/> é jsonb com o que
/// cada tipo de aviso usa (ligado, antecedência…); sem linha, valem os padrões. <see cref="LidasAte"/> marca até quando
/// os avisos do sino já foram vistos ("novo" = aviso que passou a valer depois disso).
/// </summary>
public class PreferenciaNotificacao
{
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public DateTime? LidasAte { get; set; }
    public string Config { get; set; } = "{}";
}
