using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Api.Autorizacao;
using MyFinanceAPI.Application.DTO.Notificacoes;
using MyFinanceAPI.Application.Interfaces;

namespace MyFinanceAPI.Api.Controllers
{
    /// <summary>Avisos do sino (card #73): calculados na hora; só o "lido até" é gravado.</summary>
    [Authorize(Policy = Politicas.Usuario)]
    public class NotificacoesController(INotificacaoService notificacoes) : ApiControllerBase
    {
        [HttpGet("/notificacoes")]
        public async Task<ActionResult<NotificacoesDTO>> Get() =>
            Ok(await notificacoes.ListarAsync(UsuarioId));

        /// <summary>"Marcar tudo como lido": 204.</summary>
        [HttpPost("/notificacoes/lidas")]
        public async Task<IActionResult> MarcarLidas()
        {
            await notificacoes.MarcarLidasAsync(UsuarioId);
            return NoContent();
        }
    }
}
