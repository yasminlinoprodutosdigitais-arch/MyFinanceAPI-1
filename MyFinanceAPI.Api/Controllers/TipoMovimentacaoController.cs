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
    public class TipoMovimentacaoController : ApiControllerBase
    {
        public const string RotaPorId = "GetTipoMovimentacao";

        private readonly ITipoMovimentacaoService _TipoMovimentacaoService;

        public TipoMovimentacaoController(ITipoMovimentacaoService TipoMovimentacaoService)
        {
            _TipoMovimentacaoService = TipoMovimentacaoService;
        }

        [HttpGet("/GetTipoMovimentacao")]
        public async Task<ActionResult<IEnumerable<TipoMovimentacaoDTO>>> GetTipoMovimentacao([FromQuery] Paginacao paginacao)
        {
            var userId = UsuarioId;

            var TipoMovimentacao = await _TipoMovimentacaoService.ListarAsync(userId);
            return Lista(TipoMovimentacao, paginacao);
        }

        [HttpGet("/GetTipoMovimentacaoById/{id}", Name = RotaPorId)]
        public async Task<ActionResult<TipoMovimentacaoDTO>> GetTipoMovimentacaoById(int id)
        {
            var userId = UsuarioId;

            var TipoMovimentacao = await _TipoMovimentacaoService.ObterPorIdAsync(id, userId);
            if (TipoMovimentacao is null)
                return NaoEncontrado("Tipo de movimentação não encontrado.");

            return Ok(TipoMovimentacao);
        }

        [HttpPost("/CreateTipoMovimentacao")]
        public async Task<ActionResult<TipoMovimentacaoDTO>> CreateTipoMovimentacao([FromBody] TipoMovimentacaoDTO TipoMovimentacaoDTO)
        {
            var userId = UsuarioId;

            var criado = await _TipoMovimentacaoService.AdicionarAsync(TipoMovimentacaoDTO, userId);
            return Criado(RotaPorId, criado.Id, criado);
        }

        [HttpPut("/UpdateTipoMovimentacao")]
        public async Task<ActionResult<TipoMovimentacaoDTO>> UpdateTipoMovimentacao(TipoMovimentacaoDTO TipoMovimentacaoDTO)
        {
            var userId = UsuarioId;

            await _TipoMovimentacaoService.AtualizarAsync(TipoMovimentacaoDTO, userId);
            return Ok(TipoMovimentacaoDTO);
        }

        [HttpDelete("/DeleteTipoMovimentacao/{id}")]
        public async Task<IActionResult> DeleteTipoMovimentacao(int id)
        {
            var userId = UsuarioId;

            var TipoMovimentacao = await _TipoMovimentacaoService.ObterPorIdAsync(id, userId);
            if (TipoMovimentacao == null)
                return NaoEncontrado("Tipo de movimentação não encontrado.");

            await _TipoMovimentacaoService.RemoverAsync(id, userId);
            return NoContent();
        }
    }
}
