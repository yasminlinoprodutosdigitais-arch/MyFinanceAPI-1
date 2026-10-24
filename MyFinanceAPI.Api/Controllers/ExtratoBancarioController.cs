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
        public async Task<ActionResult<IEnumerable<ExtratoBancarioDTO>>> GetExtratoBancario([FromQuery] string month, [FromQuery] Paginacao paginacao)
        {
            var userId = UsuarioId;

            var extratos = await _extratoBancarioService.GetExtratoBancario(userId, month);
            return Lista(extratos, paginacao);
        }

        // GET /GetExtratoBancarioById/123
        [HttpGet("/GetExtratoBancarioById/{id}", Name = "GetExtratoBancario")]
        public async Task<ActionResult<ExtratoBancarioDTO>> GetExtratoBancarioById(int id)
        {
            var userId = UsuarioId;

            var extrato = await _extratoBancarioService.GetExtratoBancarioById(id, userId);
            if (extrato is null)
                return NaoEncontrado("Extrato bancário não encontrado.");

            return Ok(extrato);
        }

        // POST /ImportExtratoBancario — ação de importação: 200 com o resumo (criados, ignorados…).
        [HttpPost("/ImportExtratoBancario")]
        public async Task<ActionResult<ExtratoImportacaoResultadoDTO>> ImportExtratoBancario(
            [FromForm] IFormFile arquivo,
            [FromForm] int bancoId)
        {
            var userId = UsuarioId;

            if (arquivo == null || arquivo.Length == 0)
                return ErroRequisicao("Nenhum arquivo foi enviado.");

            using var stream = arquivo.OpenReadStream();

            var resultadoImportacao = await _extratoBancarioService.ImportarExtratoAsync(
                stream,
                arquivo.FileName,
                userId,
                bancoId
            );

            return Ok(resultadoImportacao);
        }

        // DELETE /DeleteExtratoBancario/123 — inexistente: o serviço lança KeyNotFoundException → 404.
        [HttpDelete("/DeleteExtratoBancario/{id}")]
        public async Task<IActionResult> DeleteExtratoBancario(int id)
        {
            var userId = UsuarioId;

            await _extratoBancarioService.Remove(id, userId);
            return NoContent();
        }
    }
}
