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
        private readonly IImportacoesService _importacoes;

        public ExtratoBancarioController(
            IExtratoBancarioService extratoBancarioService,
            IImportacoesService importacoes)
        {
            _extratoBancarioService = extratoBancarioService;
            _importacoes = importacoes;
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

        // Card #75: histórico (meses vazio = todas), cobertura por conta e mês, vínculos e desfazer.
        [HttpGet("/GetImportacoes")]
        public async Task<ActionResult<IEnumerable<ImportacaoDTO>>> GetImportacoes([FromQuery] int? meses, [FromQuery] Paginacao paginacao) =>
            Lista(await _importacoes.ListarAsync(UsuarioId, meses), paginacao);

        [HttpGet("/GetCoberturaImportacoes")]
        public async Task<ActionResult<CoberturaImportacoesDTO>> GetCoberturaImportacoes([FromQuery] int meses = 12) =>
            Ok(await _importacoes.CoberturaAsync(UsuarioId, meses));

        [HttpGet("/GetVinculosImportacao/{id:int}")]
        public async Task<ActionResult<VinculosImportacaoDTO>> GetVinculosImportacao(int id) =>
            Ok(await _importacoes.VinculosAsync(UsuarioId, id));

        [HttpPost("/DesfazerImportacao/{id:int}")]
        public async Task<IActionResult> DesfazerImportacao(int id, [FromBody] DesfazerImportacaoDTO corpo)
        {
            await _importacoes.DesfazerAsync(UsuarioId, id, corpo.Motivo);
            return NoContent();
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
