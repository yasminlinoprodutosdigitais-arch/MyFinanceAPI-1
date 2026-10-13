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
    public class ExtratoBancarioController : ApiControllerBase
    {
        private readonly IExtratoBancarioService _extratoBancarioService;

        public ExtratoBancarioController(
            IExtratoBancarioService extratoBancarioService)
        {
            _extratoBancarioService = extratoBancarioService;
        }

        // GET /GetExtratoBancario
        [HttpGet("/GetExtratoBancario")]
        public async Task<ActionResult<IEnumerable<ExtratoBancarioDTO>>> GetExtratoBancario([FromQuery] string month)
        {
            var userId = UsuarioId;

            var extratos = await _extratoBancarioService.GetExtratoBancario(userId, month);
            return Ok(extratos);
        }

        // GET /GetExtratoBancarioById/123
        [HttpGet("/GetExtratoBancarioById/{id}", Name = "GetExtratoBancario")]
        public async Task<ActionResult<ExtratoBancarioDTO>> GetExtratoBancarioById(int id)
        {
            var userId = UsuarioId;

            var extrato = await _extratoBancarioService.GetExtratoBancarioById(id, userId);
            if (extrato is null)
                return NotFound("Extrato bancário não encontrado!");

            return Ok(extrato);
        }

        // POST /ImportExtratoBancario
        [HttpPost("/ImportExtratoBancario")]
        public async Task<IActionResult> ImportExtratoBancario(
            [FromForm] IFormFile arquivo,
            [FromForm] int bancoId)
        {
            var userId = UsuarioId;

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

        // DELETE /DeleteExtratoBancario/123
        [HttpDelete("/DeleteExtratoBancario/{id}")]
        public async Task<ActionResult<ExtratoBancarioDTO>> DeleteExtratoBancario(int id)
        {
            var userId = UsuarioId;

            var extrato = await _extratoBancarioService.GetExtratoBancarioById(id, userId);
            if (extrato == null)
                return NotFound(new { message = "Extrato bancário não encontrado." });

            await _extratoBancarioService.Remove(id, userId);
            return Ok(extrato);
        }
    }
}
