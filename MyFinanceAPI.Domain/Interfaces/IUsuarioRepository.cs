using System;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface IUsuarioRepository
{
    void Cadastrar(Usuario entidade);
    /// <summary>Usuário pelo login (UserName) ou, se não achar, pelo e-mail; null se nenhum. Não confere senha.</summary>
    public Task<Usuario?> BuscarUsuario(string login);
    public bool ValidaSeUsuarioExiste(string login, string usuarioNome);
    /// <summary>Usuário pelo e-mail, sem diferenciar maiúsculas (card #50); null se nenhum.</summary>
    Task<Usuario?> BuscarPorEmailAsync(string email);
    Task<Usuario?> ObterPorIdAsync(int id);
    /// <summary>Grava as mudanças de um usuário obtido daqui (confirmação, senha nova).</summary>
    Task AtualizarAsync(Usuario usuario);
}
