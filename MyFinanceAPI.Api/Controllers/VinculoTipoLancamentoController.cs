using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Api.Controllers
{
    [Authorize(Policy = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class VinculoTipoMovimentacaoController : ControllerBase
    {
        private readonly IVinculoTipoMovimentacaoService _service;
        private readonly IUserContextService _userContextService;

        public VinculoTipoMovimentacaoController(
            IVinculoTipoMovimentacaoService service,
            IUserContextService userContextService)
        {
            _service = service;
            _userContextService = userContextService;
        }

        [HttpGet("pendentes")]
        public async Task<IActionResult> GetPendentes()
        {
            var userId = _userContextService.GetUserIdFromClaims();
            if (userId == 0)
                return Unauthorized("Usuário não autorizado!");

            var lista = await _service.ObterPendentesAsync(userId);
            return Ok(lista);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] VinculoUpdateDTO dto)
        {
            var userId = _userContextService.GetUserIdFromClaims();
            if (userId == 0)
                return Unauthorized("Usuário não autorizado!");

            try
            {
                await _service.AtualizarVinculoAsync(id, dto, userId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
