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
    public class BancosController : ControllerBase
    {
        private readonly IBancoService _BancoService;
        private readonly IUserContextService _userContextService;

        public BancosController(IBancoService BancoService, IUserContextService userContextService)
        {
            _BancoService = BancoService;
            _userContextService = userContextService;
        }

        [HttpGet("/GetBancos")]
        public async Task<ActionResult<IEnumerable<BancoDTO>>> GetBancos()
        {
            try
            {
                int? userId = _userContextService.GetUserIdFromClaims();
                if (userId == null)
                    return Unauthorized(new { message = "Usuário não autorizado!" });

                var Bancos = await _BancoService.GetBanco(userId.Value);
                return Ok(Bancos);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Token expired or invalid.", error = ex.Message });
            }
        }


        [HttpGet("/GetBancoById/{id}", Name = "GetBanco")]
        public async Task<ActionResult<BancoDTO>> GetBancoById(int id)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                var Banco = await _BancoService.GetBancoById(id, userId);
                if (Banco is null)
                    return NotFound("Banco Bad Request");
                else
                    return Ok(Banco);

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

        [HttpPost("/CreateBanco")]
        public async Task<ActionResult> CreateBanco([FromBody] BancoDTO BancoDTO)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                if (BancoDTO is null)
                    return BadRequest("Dados inválidos.");

                await _BancoService.Add(BancoDTO, userId);
                return Ok("Banco criado com sucesso!");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("/UpdateBanco")]
        public async Task<ActionResult<BancoDTO>> UpdateBanco(BancoDTO BancoDTO)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                if (BancoDTO is null)
                    return BadRequest("Dados inválidos.");

                await _BancoService.UpdateAsync(BancoDTO, userId);
                return BancoDTO;
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("/DeleteBanco/{id}")]
        public async Task<ActionResult<BancoDTO>> DeleteBanco(int id)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                var Banco = await _BancoService.GetBancoById(id, userId);
                if (Banco == null)
                    return NotFound(new { message = "Banco não encontrado." });

                await _BancoService.Remove(id, userId);
                return Ok(Banco);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao deletar Banco.", error = ex.Message, detail = ex.StackTrace });
            }
        }
    }
}
