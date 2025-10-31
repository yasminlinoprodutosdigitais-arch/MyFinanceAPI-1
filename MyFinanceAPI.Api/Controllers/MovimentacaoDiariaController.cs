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
    public class MovimentacaoDiariaController : ControllerBase
    {
        private readonly IMovimentacaoDiariaService _MovimentacaoDiariaService;
        private readonly IUserContextService _userContextService;

        public MovimentacaoDiariaController(IMovimentacaoDiariaService MovimentacaoDiariaService, IUserContextService userContextService)
        {
            _MovimentacaoDiariaService = MovimentacaoDiariaService;
            _userContextService = userContextService;
        }

        [HttpGet("/GetMovimentacaoDiaria")]
        public async Task<ActionResult<IEnumerable<MovimentacaoDiariaDTO>>> GetMovimentacaoDiaria()
        {
            try
            {
                int? userId = _userContextService.GetUserIdFromClaims();
                if (userId == null)
                    return Unauthorized(new { message = "Usuário não autorizado!" });

                var MovimentacaoDiaria = await _MovimentacaoDiariaService.GetMovimentacaoDiaria(userId.Value);
                return Ok(MovimentacaoDiaria);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Token expired or invalid.", error = ex.Message });
            }
        }


        [HttpGet("/GetMovimentacaoDiariaById/{id}", Name = "GetMovimentacaoDiaria")]
        public async Task<ActionResult<MovimentacaoDiariaDTO>> GetMovimentacaoDiariaById(int id)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == null)
                    return Unauthorized("Usuário não autorizado!");

                var MovimentacaoDiaria = await _MovimentacaoDiariaService.GetMovimentacaoDiariaById(id, userId);
                if (MovimentacaoDiaria is null)
                    return NotFound("MovimentacaoDiaria não encontrada!");
                else
                    return Ok(MovimentacaoDiaria);

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

        [HttpPost("/CreateMovimentacaoDiaria")]
        public async Task<ActionResult> CreateMovimentacaoDiaria([FromBody] MovimentacaoDiariaDTO MovimentacaoDiariaDTO)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == null)
                    return Unauthorized("Usuário não autorizado!");

                if (MovimentacaoDiariaDTO is null)
                    return BadRequest("Dados inválidos.");

                await _MovimentacaoDiariaService.Add(MovimentacaoDiariaDTO, userId);
                return Ok("MovimentacaoDiaria criada com sucesso!");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("/UpdateMovimentacaoDiaria")]
        public async Task<ActionResult<MovimentacaoDiariaDTO>> UpdateMovimentacaoDiaria(MovimentacaoDiariaDTO MovimentacaoDiariaDTO)
        {
            var userId = _userContextService.GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized("Usuário não autorizado!");

            await _MovimentacaoDiariaService.Update(MovimentacaoDiariaDTO, userId);
            return MovimentacaoDiariaDTO;
        }

        [HttpDelete("/DeleteMovimentacaoDiaria/{id}")]
        public async Task<ActionResult<MovimentacaoDiariaDTO>> DeleteMovimentacaoDiaria(int id)
        {
            try 
            {
                var userId = _userContextService.GetUserIdFromClaims();
                    if (userId == null)
                return Unauthorized("Usuário não autorizado!");

                var MovimentacaoDiaria = await _MovimentacaoDiariaService.GetMovimentacaoDiariaById(id, userId);
                if(MovimentacaoDiaria == null)
                    return NotFound(new {message = "MovimentacaoDiaria não encontrada."});

                await _MovimentacaoDiariaService.Remove(id, userId);
                return Ok(MovimentacaoDiaria);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao deletar MovimentacaoDiaria.", error = ex.Message, detail = ex.StackTrace });
            }
        }
    }
}
