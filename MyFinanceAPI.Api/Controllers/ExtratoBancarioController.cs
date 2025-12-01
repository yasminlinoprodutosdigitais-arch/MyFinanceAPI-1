using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.DTO.Extrato;
using MyFinanceAPI.Application.DTO.Movimentacoes;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Api.Controllers
{
    [Authorize(Policy = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ExtratoBancarioController : ControllerBase
    {
        private readonly IExtratoBancarioService _extratoBancarioService;
        private readonly IUserContextService _userContextService;

        public ExtratoBancarioController(
            IExtratoBancarioService extratoBancarioService,
            IUserContextService userContextService)
        {
            _extratoBancarioService = extratoBancarioService;
            _userContextService = userContextService;
        }

        // GET /GetExtratoBancario
        [HttpGet("/GetExtratoBancario")]
        public async Task<ActionResult<IEnumerable<ExtratoBancarioDTO>>> GetExtratoBancario()
        {
            try
            {
                int? userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized(new { message = "Usuário não autorizado!" });

                var extratos = await _extratoBancarioService.GetExtratoBancario(userId.Value);
                return Ok(extratos);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Token expired or invalid.", error = ex.Message });
            }
        }

        // GET /GetExtratoBancarioById/123
        [HttpGet("/GetExtratoBancarioById/{id}", Name = "GetExtratoBancario")]
        public async Task<ActionResult<ExtratoBancarioDTO>> GetExtratoBancarioById(int id)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                var extrato = await _extratoBancarioService.GetExtratoBancarioById(id, userId);
                if (extrato is null)
                    return NotFound("Extrato bancário não encontrado!");

                return Ok(extrato);
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

        // POST /ImportExtratoBancario
        [HttpPost("/ImportExtratoBancario")]
        public async Task<IActionResult> ImportExtratoBancario(
            [FromForm] IFormFile arquivo,
            [FromForm] int bancoId)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                if (arquivo == null || arquivo.Length == 0)
                    return BadRequest("Nenhum arquivo foi enviado.");

                using var stream = arquivo.OpenReadStream();

                var resultadoImportacao = await _extratoBancarioService.ImportarExtratoAsync(
                    stream,
                    arquivo.FileName,
                    userId,
                    bancoId
                );

                return Ok(resultadoImportacao);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro ao importar extrato.",
                    error = ex.InnerException?.Message ?? ex.Message,
                });
            }
        }

        // DELETE /DeleteExtratoBancario/123
        [HttpDelete("/DeleteExtratoBancario/{id}")]
        public async Task<ActionResult<ExtratoBancarioDTO>> DeleteExtratoBancario(int id)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                var extrato = await _extratoBancarioService.GetExtratoBancarioById(id, userId);
                if (extrato == null)
                    return NotFound(new { message = "Extrato bancário não encontrado." });

                await _extratoBancarioService.Remove(id, userId);
                return Ok(extrato);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro ao deletar extrato bancário.",
                    error = ex.Message,
                    detail = ex.StackTrace
                });
            }
        }
    }
}
