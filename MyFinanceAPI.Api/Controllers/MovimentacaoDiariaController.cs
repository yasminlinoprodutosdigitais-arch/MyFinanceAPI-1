using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.DTO.Movimentacoes;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Api.Controllers
{
    [Authorize(Policy = "Admin")]
    [Route("api/[controller]")]
    public class MovimentacaoDiariaController : ApiControllerBase
    {
        private readonly IMovimentacaoDiariaService _movimentacaoDiariaService;

        public MovimentacaoDiariaController(IMovimentacaoDiariaService MovimentacaoDiariaService)
        {
            _movimentacaoDiariaService = MovimentacaoDiariaService;
        }

        [HttpGet("/GetMovimentacaoDiaria")]
        public async Task<ActionResult<IEnumerable<MovimentacaoDiariaDTO>>> GetMovimentacaoDiaria()
        {
            var userId = UsuarioId;

            var MovimentacaoDiaria = await _movimentacaoDiariaService.ListarAsync(userId);
            return Ok(MovimentacaoDiaria);
        }

        [HttpGet("/GetMovimentacaoDiariaById/{id}", Name = "GetMovimentacaoDiaria")]
        public async Task<ActionResult<MovimentacaoDiariaDTO>> GetMovimentacaoDiariaById(int id)
        {
            var userId = UsuarioId;

            var MovimentacaoDiaria = await _movimentacaoDiariaService.ObterPorIdAsync(id, userId);
            if (MovimentacaoDiaria is null)
                return NotFound("MovimentacaoDiaria não encontrada!");
            else
                return Ok(MovimentacaoDiaria);
        }

        [HttpGet("/GetMovimentacaoDiariaByDate/{date}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetMovimentacaoByDate(DateTime date)
        {
            var userId = UsuarioId;

            var transaction = await _movimentacaoDiariaService.GetMovimentacaoByDate(date, userId);
            if (transaction is null)
                return NotFound();
            else
                return Ok(transaction);
        }

        [HttpPost("/CreateMovimentacaoDiaria")]
        public async Task<ActionResult> CreateMovimentacaoDiaria([FromBody] MovimentacaoDiariaDTO MovimentacaoDiariaDTO)
        {
            var userId = UsuarioId;

            if (MovimentacaoDiariaDTO is null)
                return BadRequest("Dados inválidos.");

            await _movimentacaoDiariaService.AdicionarAsync(MovimentacaoDiariaDTO, userId);
            return Ok("MovimentacaoDiaria criada com sucesso!");
        }

        [HttpPut("/UpdateMovimentacaoDiaria")]
        public async Task<ActionResult<MovimentacaoDiariaDTO>> UpdateMovimentacaoDiaria(MovimentacaoDiariaDTO MovimentacaoDiariaDTO)
        {
            var userId = UsuarioId;

            await _movimentacaoDiariaService.AtualizarAsync(MovimentacaoDiariaDTO, userId);
            return MovimentacaoDiariaDTO;
        }

        [HttpDelete("/DeleteMovimentacaoDiaria/{id}")]
        public async Task<ActionResult<MovimentacaoDiariaDTO>> DeleteMovimentacaoDiaria(int id)
        {
            var userId = UsuarioId;

            var MovimentacaoDiaria = await _movimentacaoDiariaService.ObterPorIdAsync(id, userId);
            if (MovimentacaoDiaria == null)
                return NotFound(new { message = "MovimentacaoDiaria não encontrada." });

            await _movimentacaoDiariaService.RemoverAsync(id, userId);
            return Ok(MovimentacaoDiaria);
        }
    }
}
