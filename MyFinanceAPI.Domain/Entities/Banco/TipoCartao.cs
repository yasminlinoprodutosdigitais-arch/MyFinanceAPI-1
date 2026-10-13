using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyFinanceAPI.Domain.Entities;

/// <summary>
/// Tipo de conta/cartão. Sem usuário (UserId nulo) é padrão do sistema — "Cartão de crédito",
/// "Conta corrente" e "Conta Poupança" —, vale para todos e não pode ser editado nem excluído;
/// com usuário, é um tipo próprio dele (DB-P1-07). Por isso não herda BaseEntity, cujo UserId é obrigatório.
/// </summary>
public class TipoCartao : IEntidade
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public Usuario? User { get; set; }

    public string NomeTipoCartao { get; set; }
    public string? Descricao { get; set; }
    /// <summary>Lançamentos deste tipo são agrupados por fatura, não por data (DB-P1-07).</summary>
    public bool EhCredito { get; set; }

    public bool EhSistema => UserId == null;

    public TipoCartao() { }
}
