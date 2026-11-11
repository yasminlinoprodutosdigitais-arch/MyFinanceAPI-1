using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Application.DTO.Notificacoes;
using MyFinanceAPI.Application.Interfaces;

namespace MyFinanceAPI.Api.Controllers
{
    /// <summary>Preferências do usuário (card #73): por enquanto, só as de notificação.</summary>
    [Authorize(Policy = "Admin")]
    public class PreferenciasController(IPreferenciaNotificacaoService preferencias) : ApiControllerBase
    {
        [HttpGet("/preferencias/notificacoes")]
        public async Task<ActionResult<PreferenciasNotificacaoDTO>> GetNotificacoes() =>
            Ok(await preferencias.ObterAsync(UsuarioId));

        /// <summary>200 com o que ficou salvo; antecedência fora das opções: 400.</summary>
        [HttpPut("/preferencias/notificacoes")]
        public async Task<ActionResult<PreferenciasNotificacaoDTO>> PutNotificacoes([FromBody] PreferenciasNotificacaoDTO dto) =>
            Ok(await preferencias.SalvarAsync(UsuarioId, dto));
    }
}
