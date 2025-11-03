using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Api.Controllers
{
    [Authorize(Policy = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class TipoCartaoController : ControllerBase
    {
        private readonly ITipoCartaoService _TipoCartaoService;
        private readonly IUserContextService _userContextService;

        public TipoCartaoController(ITipoCartaoService TipoCartaoService, IUserContextService userContextService)
        {
            _TipoCartaoService = TipoCartaoService;
            _userContextService = userContextService;
        }

        [HttpGet("/GetTipoCartao")]
        public async Task<ActionResult<IEnumerable<TipoCartaoDTO>>> GetTipoCartao()
        {
            try
            {
                int? userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized(new { message = "Usuário não autorizado!" });

                var TipoCartao = await _TipoCartaoService.GetTipoCartao(userId.Value);
                return Ok(TipoCartao);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Token expired or invalid.", error = ex.Message });
            }
        }


        [HttpGet("/GetTipoCartaoById/{id}", Name = "GetTipoCartao")]
        public async Task<ActionResult<TipoCartaoDTO>> GetTipoCartaoById(int id)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                var TipoCartao = await _TipoCartaoService.GetTipoCartaoById(id, userId);
                if (TipoCartao is null)
                    return NotFound("TipoCartao não encontrada!");
                else
                    return Ok(TipoCartao);

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro inesperado.", details = ex.Message });
            }
        }

        [HttpPost("/CreateTipoCartao")]
        public async Task<ActionResult> CreateTipoCartao([FromBody] TipoCartaoDTO TipoCartaoDTO)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                if (TipoCartaoDTO is null)
                    return BadRequest("Dados inválidos.");

                await _TipoCartaoService.Add(TipoCartaoDTO, userId);
                return Ok("TipoCartao criada com sucesso!");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("/UpdateTipoCartao")]
        public async Task<ActionResult<TipoCartaoDTO>> UpdateTipoCartao(TipoCartaoDTO TipoCartaoDTO)
        {
            try
            {

                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                await _TipoCartaoService.UpdateAsync(TipoCartaoDTO, userId);
                return TipoCartaoDTO;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atualizar TipoCartao.", error = ex.Message });
            }
        }

        [HttpDelete("/DeleteTipoCartao/{id}")]
        public async Task<ActionResult<TipoCartaoDTO>> DeleteTipoCartao(int id)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                var TipoCartao = await _TipoCartaoService.GetTipoCartaoById(id, userId);
                if (TipoCartao == null)
                    return NotFound(new { message = "TipoCartao não encontrada." });

                await _TipoCartaoService.Remove(id, userId);
                return Ok(TipoCartao);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao deletar TipoCartao.", error = ex.Message, detail = ex.StackTrace });
            }
        }
    }
}
