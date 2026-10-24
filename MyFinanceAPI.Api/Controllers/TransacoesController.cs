using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Api.Controllers
{
    [Authorize(Policy = "Admin")]
    public class TransacoesController : ApiControllerBase
    {
        public const string RotaPorId = "GetTransaction";

        private readonly ITransacaoService _transactionService;

        public TransacoesController(ITransacaoService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("/GetTransactionById/{id}", Name = RotaPorId)]
        public async Task<ActionResult<TransacaoDTO>> GetTransactionById(int id)
        {
            var userId = UsuarioId;

            if (id <= 0)
                return ErroRequisicao("Id de transação inválido.");

            var transaction = await _transactionService.GetTransactionById(id, userId);

            if (transaction == null)
                return NaoEncontrado("Transação não encontrada.");

            return Ok(transaction);
        }

        [HttpGet("/GetTransactionByDate/{date}/{pesquisaDataCompleta}")]
        public async Task<ActionResult<IEnumerable<TransacaoDTO>>> GetTransactionByDate(DateTime date, bool pesquisaDataCompleta, [FromQuery] Paginacao paginacao)
        {
            var userId = UsuarioId;

            var transaction = await _transactionService.GetTransactionByDate(date, userId, pesquisaDataCompleta);
            return Lista(transaction, paginacao);
        }

        [HttpGet("/GetContaVencida/{date}")]
        public async Task<ActionResult<IEnumerable<TransacaoDTO>>> GetContaVencida(DateTime date, [FromQuery] Paginacao paginacao)
        {
            var userId = UsuarioId;

            var transaction = await _transactionService.GetContaVencida(date, userId);
            return Lista(transaction, paginacao);
        }

        [HttpGet("/BuscarContasMensais/{date}/{pesquisaDataCompleta}")]
        public async Task<ActionResult<IEnumerable<TransacaoDTO>>> BuscarContasMensais(DateTime date, bool pesquisaDataCompleta, [FromQuery] Paginacao paginacao)
        {
            var userId = UsuarioId;

            var transaction = await _transactionService.BuscarContasMensais(date, userId, pesquisaDataCompleta);
            return Lista(transaction, paginacao);
        }

        /// <summary>
        /// Cria uma transação por vencimento da conta: o 201 leva todas as criadas, com o Location da primeira.
        /// </summary>
        [HttpPost("/CreateTransaction")]
        public async Task<ActionResult<IReadOnlyList<TransacaoDTO>>> CreateTransaction([FromBody] TransacaoDTO transactionDTO)
        {
            var userId = UsuarioId;

            var criadas = await _transactionService.Add(transactionDTO, userId);
            return Criado(RotaPorId, criadas[0].Id, criadas);
        }

        [HttpPut("/UpdateTransaction")]
        public async Task<ActionResult<TransacaoDTO>> UpdateTransaction([FromBody] TransacaoDTO transactionDTO)
        {
            var userId = UsuarioId;

            await _transactionService.Update(transactionDTO, userId);
            return Ok(transactionDTO);
        }

        // Inexistente: o serviço lança NaoEncontradoException → 404.
        [HttpDelete("/DeleteTransaction/{id}")]
        public async Task<IActionResult> DeleteAccountHistory(int id)
        {
            var userId = UsuarioId;

            await _transactionService.Delete(id, userId);
            return NoContent();
        }
    }
}
