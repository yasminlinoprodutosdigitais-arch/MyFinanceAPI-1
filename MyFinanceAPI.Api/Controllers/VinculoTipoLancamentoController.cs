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

        public VinculoTipoMovimentacaoController(IVinculoTipoMovimentacaoService service)
        {
            _service = service;
        }

        [HttpGet("pendentes")]
        public async Task<IActionResult> GetPendentes()
        {
            int userId = 1; // pegar do token depois
            var lista = await _service.ObterPendentesAsync(userId);
            return Ok(lista);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] VinculoUpdateDTO dto)
        {
            int userId = 1;
            await _service.AtualizarVinculoAsync(id, dto, userId);
            return NoContent();
        }
    }
}