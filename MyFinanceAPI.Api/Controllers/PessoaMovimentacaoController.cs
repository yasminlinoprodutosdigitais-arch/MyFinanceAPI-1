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
    public class PessoaMovimentacaoController : ApiControllerBase
    {
        private readonly IPessoaMovimentacaoService _PessoaMovimentacaoService;

        public PessoaMovimentacaoController(IPessoaMovimentacaoService PessoaMovimentacaoService)
        {
            _PessoaMovimentacaoService = PessoaMovimentacaoService;
        }

        [HttpGet("/GetPessoaMovimentacao")]
        public async Task<ActionResult<IEnumerable<PessoaMovimentacaoDTO>>> GetPessoaMovimentacao()
        {
            var userId = UsuarioId;

            var PessoaMovimentacao = await _PessoaMovimentacaoService.ListarAsync(userId);
            return Ok(PessoaMovimentacao);
        }

        [HttpGet("/GetPessoaMovimentacaoById/{id}", Name = "GetPessoaMovimentacao")]
        public async Task<ActionResult<PessoaMovimentacaoDTO>> GetPessoaMovimentacaoById(int id)
        {
            var userId = UsuarioId;

            var PessoaMovimentacao = await _PessoaMovimentacaoService.ObterPorIdAsync(id, userId);
            if (PessoaMovimentacao is null)
                return NotFound("PessoaMovimentacao não encontrada!");
            else
                return Ok(PessoaMovimentacao);

        }

        [HttpPost("/CreatePessoaMovimentacao")]
        public async Task<ActionResult> CreatePessoaMovimentacao([FromBody] PessoaMovimentacaoDTO dto)
        {
            var userId = UsuarioId;

            if (dto is null)
                return BadRequest(new { message = "Dados inválidos." });

            await _PessoaMovimentacaoService.AdicionarAsync(dto, userId);

            return Ok(new { success = true, message = "PessoaMovimentacao criada com sucesso!" });
        }

        [HttpPut("/UpdatePessoaMovimentacao")]
        public async Task<ActionResult<PessoaMovimentacaoDTO>> UpdatePessoaMovimentacao(PessoaMovimentacaoDTO PessoaMovimentacaoDTO)
        {
            var userId = UsuarioId;

            await _PessoaMovimentacaoService.AtualizarAsync(PessoaMovimentacaoDTO, userId);
            return PessoaMovimentacaoDTO;
        }

        [HttpDelete("/DeletePessoaMovimentacao/{id}")]
        public async Task<ActionResult<PessoaMovimentacaoDTO>> DeletePessoaMovimentacao(int id)
        {
            var userId = UsuarioId;

            var PessoaMovimentacao = await _PessoaMovimentacaoService.ObterPorIdAsync(id, userId);
            if (PessoaMovimentacao == null)
                return NotFound(new { message = "PessoaMovimentacao não encontrada." });

            await _PessoaMovimentacaoService.RemoverAsync(id, userId);
            return Ok(PessoaMovimentacao);
        }
    }
}
