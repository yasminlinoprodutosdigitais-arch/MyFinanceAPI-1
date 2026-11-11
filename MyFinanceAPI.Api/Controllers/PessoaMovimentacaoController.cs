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
    public class PessoaMovimentacaoController : ApiControllerBase
    {
        public const string RotaPorId = "GetPessoaMovimentacao";

        private readonly IPessoaMovimentacaoService _PessoaMovimentacaoService;

        public PessoaMovimentacaoController(IPessoaMovimentacaoService PessoaMovimentacaoService)
        {
            _PessoaMovimentacaoService = PessoaMovimentacaoService;
        }

        [HttpGet("/GetPessoaMovimentacao")]
        public async Task<ActionResult<IEnumerable<PessoaMovimentacaoDTO>>> GetPessoaMovimentacao([FromQuery] Paginacao paginacao)
        {
            var userId = UsuarioId;

            var PessoaMovimentacao = await _PessoaMovimentacaoService.ListarAsync(userId);
            return Lista(PessoaMovimentacao, paginacao);
        }

        [HttpGet("/GetPessoaMovimentacaoById/{id}", Name = RotaPorId)]
        public async Task<ActionResult<PessoaMovimentacaoDTO>> GetPessoaMovimentacaoById(int id)
        {
            var userId = UsuarioId;

            var PessoaMovimentacao = await _PessoaMovimentacaoService.ObterPorIdAsync(id, userId);
            if (PessoaMovimentacao is null)
                return NaoEncontrado("Pessoa não encontrada.");

            return Ok(PessoaMovimentacao);
        }

        [HttpPost("/CreatePessoaMovimentacao")]
        public async Task<ActionResult<PessoaMovimentacaoDTO>> CreatePessoaMovimentacao([FromBody] PessoaMovimentacaoDTO dto)
        {
            var userId = UsuarioId;

            var criada = await _PessoaMovimentacaoService.AdicionarAsync(dto, userId);
            return Criado(RotaPorId, criada.Id, criada);
        }

        [HttpPut("/UpdatePessoaMovimentacao")]
        public async Task<ActionResult<PessoaMovimentacaoDTO>> UpdatePessoaMovimentacao(PessoaMovimentacaoDTO PessoaMovimentacaoDTO)
        {
            var userId = UsuarioId;

            await _PessoaMovimentacaoService.AtualizarAsync(PessoaMovimentacaoDTO, userId);
            return Ok(PessoaMovimentacaoDTO);
        }

        [HttpDelete("/DeletePessoaMovimentacao/{id}")]
        public async Task<IActionResult> DeletePessoaMovimentacao(int id)
        {
            var userId = UsuarioId;

            var PessoaMovimentacao = await _PessoaMovimentacaoService.ObterPorIdAsync(id, userId);
            if (PessoaMovimentacao == null)
                return NaoEncontrado("Pessoa não encontrada.");

            await _PessoaMovimentacaoService.RemoverAsync(id, userId);
            return NoContent();
        }
    }
}
