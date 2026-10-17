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
    /// <summary>
    /// Lançamentos diários que mexem no saldo do banco (a regra está no MovimentacaoDiariaService).
    /// Nenhuma tela usa este módulo hoje — a tela de movimentações usa ExtratoBancario —, mas ele fica
    /// por decisão de 14/10/2026 (BE-P3-02): é onde vive a lógica de saldo bancário.
    /// </summary>
    [Authorize(Policy = "Admin")]
    public class MovimentacaoDiariaController : ApiControllerBase
    {
        public const string RotaPorId = "GetMovimentacaoDiaria";

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

        [HttpGet("/GetMovimentacaoDiariaById/{id}", Name = RotaPorId)]
        public async Task<ActionResult<MovimentacaoDiariaDTO>> GetMovimentacaoDiariaById(int id)
        {
            var userId = UsuarioId;

            var MovimentacaoDiaria = await _movimentacaoDiariaService.ObterPorIdAsync(id, userId);
            if (MovimentacaoDiaria is null)
                return NaoEncontrado("Movimentação diária não encontrada.");

            return Ok(MovimentacaoDiaria);
        }

        [HttpGet("/GetMovimentacaoDiariaByDate/{date}")]
        public async Task<ActionResult<IEnumerable<MovimentacaoDiariaDTO>>> GetMovimentacaoByDate(DateTime date)
        {
            var userId = UsuarioId;

            var movimentacoes = await _movimentacaoDiariaService.GetMovimentacaoByDate(date, userId);
            return Ok(movimentacoes);
        }

        [HttpPost("/CreateMovimentacaoDiaria")]
        public async Task<ActionResult<MovimentacaoDiariaDTO>> CreateMovimentacaoDiaria([FromBody] MovimentacaoDiariaDTO MovimentacaoDiariaDTO)
        {
            var userId = UsuarioId;

            var criada = await _movimentacaoDiariaService.AdicionarAsync(MovimentacaoDiariaDTO, userId);
            return Criado(RotaPorId, criada.Id, criada);
        }

        [HttpPut("/UpdateMovimentacaoDiaria")]
        public async Task<ActionResult<MovimentacaoDiariaDTO>> UpdateMovimentacaoDiaria(MovimentacaoDiariaDTO MovimentacaoDiariaDTO)
        {
            var userId = UsuarioId;

            await _movimentacaoDiariaService.AtualizarAsync(MovimentacaoDiariaDTO, userId);
            return Ok(MovimentacaoDiariaDTO);
        }

        [HttpDelete("/DeleteMovimentacaoDiaria/{id}")]
        public async Task<IActionResult> DeleteMovimentacaoDiaria(int id)
        {
            var userId = UsuarioId;

            var MovimentacaoDiaria = await _movimentacaoDiariaService.ObterPorIdAsync(id, userId);
            if (MovimentacaoDiaria == null)
                return NaoEncontrado("Movimentação diária não encontrada.");

            await _movimentacaoDiariaService.RemoverAsync(id, userId);
            return NoContent();
        }
    }
}
