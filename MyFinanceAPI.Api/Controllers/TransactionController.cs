using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Api.Controllers
{
    [Authorize(Policy = "Admin")]
    public class TransactionController : ApiControllerBase
    {
        public const string RotaPorId = "GetTransaction";

        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("/GetTransactionById/{id}", Name = RotaPorId)]
        public async Task<ActionResult<TransactionDTO>> GetTransactionById(int id)
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
        public async Task<ActionResult<IEnumerable<TransactionDTO>>> GetTransactionByDate(DateTime date, bool pesquisaDataCompleta)
        {
            var userId = UsuarioId;

            var transaction = await _transactionService.GetTransactionByDate(date, userId, pesquisaDataCompleta);
            return Ok(transaction);
        }

        [HttpGet("/GetContaVencida/{date}")]
        public async Task<ActionResult<IEnumerable<TransactionDTO>>> GetContaVencida(DateTime date)
        {
            var userId = UsuarioId;

            var transaction = await _transactionService.GetContaVencida(date, userId);
            return Ok(transaction);
        }

        [HttpGet("/BuscarContasMensais/{date}/{pesquisaDataCompleta}")]
        public async Task<ActionResult<IEnumerable<TransactionDTO>>> BuscarContasMensais(DateTime date, bool pesquisaDataCompleta)
        {
            var userId = UsuarioId;

            var transaction = await _transactionService.BuscarContasMensais(date, userId, pesquisaDataCompleta);
            return Ok(transaction);
        }

        /// <summary>
        /// Cria uma transação por vencimento da conta: o 201 leva todas as criadas, com o Location da primeira.
        /// </summary>
        [HttpPost("/CreateTransaction")]
        public async Task<ActionResult<IReadOnlyList<TransactionDTO>>> CreateTransaction([FromBody] TransactionDTO transactionDTO)
        {
            var userId = UsuarioId;

            var criadas = await _transactionService.Add(transactionDTO, userId);
            return Criado(RotaPorId, criadas[0].Id, criadas);
        }

        [HttpPut("/UpdateTransaction")]
        public async Task<ActionResult<TransactionDTO>> UpdateTransaction([FromBody] TransactionDTO transactionDTO)
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
