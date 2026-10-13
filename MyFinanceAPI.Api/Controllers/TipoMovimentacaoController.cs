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
    public class TipoMovimentacaoController : ApiControllerBase
    {
        private readonly ITipoMovimentacaoService _TipoMovimentacaoService;

        public TipoMovimentacaoController(ITipoMovimentacaoService TipoMovimentacaoService)
        {
            _TipoMovimentacaoService = TipoMovimentacaoService;
        }

        [HttpGet("/GetTipoMovimentacao")]
        public async Task<ActionResult<IEnumerable<TipoMovimentacaoDTO>>> GetTipoMovimentacao()
        {
            var userId = UsuarioId;

            var TipoMovimentacao = await _TipoMovimentacaoService.ListarAsync(userId);
            return Ok(TipoMovimentacao);
        }

        [HttpGet("/GetTipoMovimentacaoById/{id}", Name = "GetTipoMovimentacao")]
        public async Task<ActionResult<TipoMovimentacaoDTO>> GetTipoMovimentacaoById(int id)
        {
            var userId = UsuarioId;

            var TipoMovimentacao = await _TipoMovimentacaoService.ObterPorIdAsync(id, userId);
            if (TipoMovimentacao is null)
                return NotFound("Tipo Movimentacao não encontrada!");
            else
                return Ok(TipoMovimentacao);

        }

        [HttpPost("/CreateTipoMovimentacao")]
        public async Task<ActionResult> CreateTipoMovimentacao([FromBody] TipoMovimentacaoDTO TipoMovimentacaoDTO)
        {
            var userId = UsuarioId;

            if (TipoMovimentacaoDTO is null)
                return BadRequest("Dados inválidos.");

            await _TipoMovimentacaoService.AdicionarAsync(TipoMovimentacaoDTO, userId);
            return Ok(new { success = true, message = "Tipo Movimentacao criada com sucesso!" });
        }

        [HttpPut("/UpdateTipoMovimentacao")]
        public async Task<ActionResult<TipoMovimentacaoDTO>> UpdateTipoMovimentacao(TipoMovimentacaoDTO TipoMovimentacaoDTO)
        {
            var userId = UsuarioId;

            await _TipoMovimentacaoService.AtualizarAsync(TipoMovimentacaoDTO, userId);
            return TipoMovimentacaoDTO;
        }

        [HttpDelete("/DeleteTipoMovimentacao/{id}")]
        public async Task<ActionResult<TipoMovimentacaoDTO>> DeleteTipoMovimentacao(int id)
        {
            var userId = UsuarioId;

            var TipoMovimentacao = await _TipoMovimentacaoService.ObterPorIdAsync(id, userId);
            if(TipoMovimentacao == null)
                return NotFound(new {message = "Tipo Movimentacao não encontrada."});

            await _TipoMovimentacaoService.RemoverAsync(id, userId);
            return Ok(TipoMovimentacao);
        }
    }
}
