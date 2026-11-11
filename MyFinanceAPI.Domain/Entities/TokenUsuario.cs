namespace MyFinanceAPI.Domain.Entities;

public enum TipoTokenUsuario
{
    ConfirmarEmail = 1,
    RedefinirSenha = 2,
}

/// <summary>
/// Token dos links enviados por e-mail (card #50). Guarda só o hash (o token existe só no link), vale uma vez
/// e vence em <see cref="ExpiraEm"/>. <see cref="UsadoEm"/> também marca o token substituído por um mais novo.
/// Próprio, e não o do Identity: as chaves de Data Protection se perdem a cada instância do Cloud Run.
/// </summary>
public class TokenUsuario
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public TipoTokenUsuario Tipo { get; set; }
    public string Hash { get; set; } = null!;
    public DateTime ExpiraEm { get; set; }
    public DateTime? UsadoEm { get; set; }
}
