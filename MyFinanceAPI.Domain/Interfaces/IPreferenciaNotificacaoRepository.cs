using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

/// <summary>Preferências de notificação (card #73): uma linha por usuário.</summary>
public interface IPreferenciaNotificacaoRepository
{
    /// <summary>Null = o usuário nunca salvou nem marcou nada como lido (valem os padrões).</summary>
    Task<PreferenciaNotificacao?> ObterAsync(int userId);
    /// <summary>Grava a configuração (jsonb), criando a linha se não houver.</summary>
    Task SalvarConfigAsync(int userId, string config);
    /// <summary>Grava o "lido até", criando a linha se não houver.</summary>
    Task MarcarLidasAsync(int userId, DateTime lidasAte);
}
