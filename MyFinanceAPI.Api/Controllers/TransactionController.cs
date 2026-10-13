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
    [Route("api/[controller]")]
    public class TransactionController : ApiControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("/GetTransactions")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            var userId = UsuarioId;

            var update = await _transactionService.GetTransactions(userId);
            if (update is null)
                return NotFound("");
            else
                return Ok(update);

        }

        [HttpGet("/GetTransactionById/{id}", Name = "GetUpdate")]
        public async Task<ActionResult<Transaction>> GetTransactionById(int id)
        {
            var userId = UsuarioId;

            if (id <= 0)
                return BadRequest(new { message = "Invalid transaction ID." });

            var transaction = await _transactionService.GetTransactionById(id, userId);

            if (transaction == null)
                return NotFound(new { message = "Transaction not found." });

            return Ok(transaction);
        }

        [HttpGet("/GetTransactionByDate/{date}/{pesquisaDataCompleta}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionByDate(DateTime date, bool pesquisaDataCompleta)
        {
            var userId = UsuarioId;

            var transaction = await _transactionService.GetTransactionByDate(date, userId, pesquisaDataCompleta);
            if (transaction is null)
                return NotFound();
            else
                return Ok(transaction);
        }

        [HttpGet("/GetContaVencida/{date}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetContaVencida(DateTime date)
        {
            var userId = UsuarioId;

            var transaction = await _transactionService.GetContaVencida(date, userId);
            if (transaction is null)
                return NotFound();
            else
                return Ok(transaction);
        }

        [HttpGet("/GetTransactionGroupingByDate/{date}/{pesquisaDataCompleta}")]
        public async Task<ActionResult<IEnumerable<TransactionDTO>>> GetTransactionGroupingByDate(DateTime date, bool pesquisaDataCompleta)
        {
            var userId = UsuarioId;

            var transaction = await _transactionService.GetTransactionGroupingByDate(date, userId, pesquisaDataCompleta);
            if (transaction is null)
                return NotFound();
            else
                return Ok(transaction);
        }

        [HttpGet("/BuscarContasMensais/{date}/{pesquisaDataCompleta}")]
        public async Task<ActionResult<IEnumerable<TransactionDTO>>> BuscarContasMensais(DateTime date, bool pesquisaDataCompleta)
        {
            var userId = UsuarioId;

            var transaction = await _transactionService.BuscarContasMensais(date, userId, pesquisaDataCompleta);
            if (transaction is null)
                return NotFound();
            else
                return Ok(transaction);
        }

        [HttpPost("/CreateTransaction")]
        public async Task<ActionResult<TransactionDTO>> CreateTransaction([FromBody] TransactionDTO transactionDTO)
        {
            var userId = UsuarioId;

            await _transactionService.Add(transactionDTO, userId);
            return new CreatedAtRouteResult("Getupdate", new { id = transactionDTO.Id }, transactionDTO);
        }

        [HttpPut("/UpdateTransaction")]
        public async Task<ActionResult<TransactionDTO>> UpdateTransaction([FromBody] TransactionDTO transactionDTO)
        {
            var userId = UsuarioId;

            await _transactionService.Update(transactionDTO, userId);
            return transactionDTO;
        }

        [HttpDelete("/DeleteTransaction/{id}")]
        public async Task<ActionResult<TransactionDTO>> DeleteAccountHistory(int id)
        {
            var userId = UsuarioId;

            var transaction = await _transactionService.GetTransactionById(id, userId);
            await _transactionService.Delete(id, userId);
            return transaction;
        }
    }
}
