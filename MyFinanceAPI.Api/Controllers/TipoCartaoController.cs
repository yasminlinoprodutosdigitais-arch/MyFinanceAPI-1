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
    public class TipoCartaoController : ApiControllerBase
    {
        private readonly ITipoCartaoService _TipoCartaoService;

        public TipoCartaoController(ITipoCartaoService TipoCartaoService)
        {
            _TipoCartaoService = TipoCartaoService;
        }

        [HttpGet("/GetTipoCartao")]
        public async Task<ActionResult<IEnumerable<TipoCartaoDTO>>> GetTipoCartao()
        {
            var userId = UsuarioId;

            var TipoCartao = await _TipoCartaoService.ListarAsync(userId);
            return Ok(TipoCartao);
        }

        [HttpGet("/GetTipoCartaoById/{id}", Name = "GetTipoCartao")]
        public async Task<ActionResult<TipoCartaoDTO>> GetTipoCartaoById(int id)
        {
            var userId = UsuarioId;

            var TipoCartao = await _TipoCartaoService.ObterPorIdAsync(id, userId);
            if (TipoCartao is null)
                return NotFound("TipoCartao não encontrada!");
            else
                return Ok(TipoCartao);

        }

        [HttpPost("/CreateTipoCartao")]
        public async Task<ActionResult> CreateTipoCartao([FromBody] TipoCartaoDTO dto)
        {
            var userId = UsuarioId;

            if (dto is null)
                return BadRequest(new { message = "Dados inválidos." });

            await _TipoCartaoService.AdicionarAsync(dto, userId);

            return Ok(new { success = true, message = "TipoCartao criada com sucesso!" });
        }

        [HttpPut("/UpdateTipoCartao")]
        public async Task<ActionResult<TipoCartaoDTO>> UpdateTipoCartao(TipoCartaoDTO TipoCartaoDTO)
        {
            var userId = UsuarioId;

            await _TipoCartaoService.AtualizarAsync(TipoCartaoDTO, userId);
            return TipoCartaoDTO;
        }

        [HttpDelete("/DeleteTipoCartao/{id}")]
        public async Task<ActionResult<TipoCartaoDTO>> DeleteTipoCartao(int id)
        {
            var userId = UsuarioId;

            var TipoCartao = await _TipoCartaoService.ObterPorIdAsync(id, userId);
            if (TipoCartao == null)
                return NotFound(new { message = "TipoCartao não encontrada." });

            await _TipoCartaoService.RemoverAsync(id, userId);
            return Ok(TipoCartao);
        }
    }
}
