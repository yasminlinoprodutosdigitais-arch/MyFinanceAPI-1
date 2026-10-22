using System;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceAPI.Domain.Entities;

/// <summary>
/// Tipo de conta/cartão. Sem usuário (UsuarioId nulo) é padrão do sistema — "Cartão de crédito",
/// "Conta corrente" e "Conta Poupança" —, vale para todos e não pode ser editado nem excluído;
/// com usuário, é um tipo próprio dele (DB-P1-07). Por isso não herda BaseEntity, cujo UsuarioId é obrigatório.
/// </summary>
public class TipoCartao : IEntidade
{
    public int Id { get; set; }
    public int? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public string NomeTipoCartao { get; set; }
    public string? Descricao { get; set; }
    /// <summary>Lançamentos deste tipo são agrupados por fatura, não por data (DB-P1-07).</summary>
    public bool EhCredito { get; set; }

    public bool EhSistema => UsuarioId == null;

    public TipoCartao() { }
}
