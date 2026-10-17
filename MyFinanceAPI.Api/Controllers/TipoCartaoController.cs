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
    public class TipoCartaoController : ApiControllerBase
    {
        public const string RotaPorId = "GetTipoCartao";

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

        [HttpGet("/GetTipoCartaoById/{id}", Name = RotaPorId)]
        public async Task<ActionResult<TipoCartaoDTO>> GetTipoCartaoById(int id)
        {
            var userId = UsuarioId;

            var TipoCartao = await _TipoCartaoService.ObterPorIdAsync(id, userId);
            if (TipoCartao is null)
                return NaoEncontrado("Tipo de cartão não encontrado.");

            return Ok(TipoCartao);
        }

        [HttpPost("/CreateTipoCartao")]
        public async Task<ActionResult<TipoCartaoDTO>> CreateTipoCartao([FromBody] TipoCartaoDTO dto)
        {
            var userId = UsuarioId;

            var criado = await _TipoCartaoService.AdicionarAsync(dto, userId);
            return Criado(RotaPorId, criado.Id, criado);
        }

        [HttpPut("/UpdateTipoCartao")]
        public async Task<ActionResult<TipoCartaoDTO>> UpdateTipoCartao(TipoCartaoDTO TipoCartaoDTO)
        {
            var userId = UsuarioId;

            await _TipoCartaoService.AtualizarAsync(TipoCartaoDTO, userId);
            return Ok(TipoCartaoDTO);
        }

        [HttpDelete("/DeleteTipoCartao/{id}")]
        public async Task<IActionResult> DeleteTipoCartao(int id)
        {
            var userId = UsuarioId;

            var TipoCartao = await _TipoCartaoService.ObterPorIdAsync(id, userId);
            if (TipoCartao == null)
                return NaoEncontrado("Tipo de cartão não encontrado.");

            await _TipoCartaoService.RemoverAsync(id, userId);
            return NoContent();
        }
    }
}
