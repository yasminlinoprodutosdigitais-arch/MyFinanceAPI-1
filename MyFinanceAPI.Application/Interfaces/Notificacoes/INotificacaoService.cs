using MyFinanceAPI.Application.DTO.Notificacoes;

namespace MyFinanceAPI.Application.Interfaces;

/// <summary>Avisos do sino (card #73), calculados a partir dos dados de hoje e das preferências.</summary>
public interface INotificacaoService
{
    Task<NotificacoesDTO> ListarAsync(int userId);
    /// <summary>"Marcar tudo como lido": grava o "lido até" = agora.</summary>
    Task MarcarLidasAsync(int userId);
}
