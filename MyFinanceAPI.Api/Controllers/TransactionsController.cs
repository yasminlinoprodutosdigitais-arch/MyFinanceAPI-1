using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("/CreateTransaction")]
        public async Task<ActionResult<TransactionDTO>> CreateTransaction([FromBody] TransactionDTO transactionDTO)
        {
            if(transactionDTO is null)
                return BadRequest("Transaction Bad Request");
            
            await _transactionService.Add(transactionDTO);

            return new CreatedAtRouteResult("GetTransaction", new {id = transactionDTO.Id}, transactionDTO);
        }

        [HttpGet("/UpdateTransaction")]
        public async Task<ActionResult<TransactionDTO>> UpdateTransaction(TransactionDTO transactionDTO)
        {
            await _transactionService.Update(transactionDTO);
            return transactionDTO;
        }

        [HttpGet("/GetTransactions")]
        public async Task<ActionResult<IEnumerable<TransactionDTO>>> GetTransactions()
        {
            try
            {
                var transaction = await _transactionService.GetTransactions();
                return Ok(transaction);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("GetTransacionById/{id}", Name = "GetTransaction")]
        public async Task<ActionResult<TransactionDTO>> GetTransactioById(int id)
        {
            var transaction = await _transactionService.GetTransactionById(id);
            if(transaction is null)
                return NotFound("Transaction Bad Request");
            else 
                return Ok(transaction); 
        }

        [HttpGet("/GetTransactionsByCategory/{id}")]
        public async Task<ActionResult<ActionResult<TransactionDTO>>> GetTransactionByCategory(int idCategory)
        {
            if(idCategory == 0 || idCategory < 0)
                return BadRequest("");

            var transactions = await _transactionService.GetTransactionByCategory(idCategory);

            if(transactions is null)
                return NotFound();
            else
                return Ok(transactions);
        }
    }
}
