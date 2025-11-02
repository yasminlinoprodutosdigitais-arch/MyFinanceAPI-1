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
    public class TipoMovimentacaoController : ControllerBase
    {
        private readonly ITipoMovimentacaoService _TipoMovimentacaoService;
        private readonly IUserContextService _userContextService;

        public TipoMovimentacaoController(ITipoMovimentacaoService TipoMovimentacaoService, IUserContextService userContextService)
        {
            _TipoMovimentacaoService = TipoMovimentacaoService;
            _userContextService = userContextService;
        }

        [HttpGet("/GetTipoMovimentacao")]
        public async Task<ActionResult<IEnumerable<TipoMovimentacaoDTO>>> GetTipoMovimentacao()
        {
            try
            {
                int? userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized(new { message = "Usuário não autorizado!" });

                var TipoMovimentacao = await _TipoMovimentacaoService.GetTipoMovimentacao(userId.Value);
                return Ok(TipoMovimentacao);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Token expired or invalid.", error = ex.Message });
            }
        }


        [HttpGet("/GetTipoMovimentacaoById/{id}", Name = "GetTipoMovimentacao")]
        public async Task<ActionResult<TipoMovimentacaoDTO>> GetTipoMovimentacaoById(int id)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                var TipoMovimentacao = await _TipoMovimentacaoService.GetTipoMovimentacaoById(id, userId);
                if (TipoMovimentacao is null)
                    return NotFound("Tipo Movimentacao não encontrada!");
                else
                    return Ok(TipoMovimentacao);

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

        [HttpPost("/CreateTipoMovimentacao")]
        public async Task<ActionResult> CreateTipoMovimentacao([FromBody] TipoMovimentacaoDTO TipoMovimentacaoDTO)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                if (TipoMovimentacaoDTO is null)
                    return BadRequest("Dados inválidos.");

                await _TipoMovimentacaoService.Add(TipoMovimentacaoDTO, userId);
                return Ok("Tipo Movimentacao criada com sucesso!");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("/UpdateTipoMovimentacao")]
        public async Task<ActionResult<TipoMovimentacaoDTO>> UpdateTipoMovimentacao(TipoMovimentacaoDTO TipoMovimentacaoDTO)
        {
            var userId = _userContextService.GetUserIdFromClaims();
            if (userId == 0)
                return Unauthorized("Usuário não autorizado!");

            await _TipoMovimentacaoService.UpdateAsync(TipoMovimentacaoDTO, userId);
            return TipoMovimentacaoDTO;
        }

        [HttpDelete("/DeleteTipoMovimentacao/{id}")]
        public async Task<ActionResult<TipoMovimentacaoDTO>> DeleteTipoMovimentacao(int id)
        {
            try 
            {
                var userId = _userContextService.GetUserIdFromClaims();
                    if (userId == 0)
                return Unauthorized("Usuário não autorizado!");

                var TipoMovimentacao = await _TipoMovimentacaoService.GetTipoMovimentacaoById(id, userId);
                if(TipoMovimentacao == null)
                    return NotFound(new {message = "Tipo Movimentacao não encontrada."});

                await _TipoMovimentacaoService.Remove(id, userId);
                return Ok(TipoMovimentacao);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao deletar Tipo Movimentacao.", error = ex.Message, detail = ex.StackTrace });
            }
        }
    }
}
