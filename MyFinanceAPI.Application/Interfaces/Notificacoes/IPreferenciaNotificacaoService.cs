using MyFinanceAPI.Application.DTO.Notificacoes;

namespace MyFinanceAPI.Application.Interfaces;

/// <summary>Preferências de notificação do usuário (card #73).</summary>
public interface IPreferenciaNotificacaoService
{
    /// <summary>As salvas, com os padrões no que faltar.</summary>
    Task<PreferenciasNotificacaoDTO> ObterAsync(int userId);
    /// <summary>Valida (antecedências só entre as opções da tela) e grava; devolve o que ficou salvo.</summary>
    Task<PreferenciasNotificacaoDTO> SalvarAsync(int userId, PreferenciasNotificacaoDTO dto);
}
