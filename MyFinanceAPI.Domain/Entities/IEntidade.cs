namespace MyFinanceAPI.Domain.Entities;

/// <summary>
/// Registro com chave inteira — o mínimo que a base dos cadastros (RepositorioDoUsuario) precisa.
/// O dono fica de fora porque o TipoCartao tem UserId opcional (padrão do sistema).
/// </summary>
public interface IEntidade
{
    int Id { get; set; }
}
