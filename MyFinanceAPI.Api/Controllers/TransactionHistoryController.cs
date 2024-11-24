using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionHistoryController : ControllerBase
    {
        private readonly ITransactionHistoryService _transactionHistoryService;

        public TransactionHistoryController(ITransactionHistoryService transactionHistoryService)
        {
            _transactionHistoryService = transactionHistoryService;
        }

        [HttpGet("/GetTransactionHistory")]
        public async Task<ActionResult<IEnumerable<TransactionHistory>>> GetTransactionHistory()
        {
            var transactions = await _transactionHistoryService.GetTransactionHistory();
            if(transactions is null)
                return NotFound("");
            else
                return Ok(transactions);
        }

        [HttpGet("/GetByIdCategory/{id}")]
        public async Task<ActionResult<IEnumerable<TransactionHistory>>> GetTransactionHistoryByCategory(int idCategory)
        {
            if(idCategory == 0 || idCategory < 0)
                return BadRequest("");

            var transactions = await _transactionHistoryService.GetTransactionHistoryByCategory(idCategory);

            if(transactions is null)
                return NotFound();
            else
                return Ok(transactions);
        }

        [HttpGet("/GetById/{id}")]
        public async Task<ActionResult<TransactionHistory>> GetHistoryById(int id)
        {
            if(id == 0 || id < 0)
                return BadRequest();
            var transaction = await _transactionHistoryService.GetTransactionHistoryById(id);

            if(transaction is null)
                return NotFound();
            else
                return Ok(transaction);
        }

        [HttpGet("/GetByDate/{date}")]
        public async Task<ActionResult<IEnumerable<TransactionHistory>>> GetTransactionHistoryByDate(DateTime date)
        {
            var transactions = await _transactionHistoryService.GetTransactionHistoryByDate(date);
            if(transactions is null)
                return NotFound();
            else
                return Ok(transactions);
        }

        [HttpPost("/CreateTransactionHistory")]
        public async Task<ActionResult<TransactionHistoryDTO>> CreateTransactionHistory([FromBody] TransactionHistoryDTO transactionHistoryDTO)
        {
            await _transactionHistoryService.Add(transactionHistoryDTO);
            return new CreatedAtRouteResult("GetTransaction", new {id = transactionHistoryDTO.Id}, transactionHistoryDTO);
        }

        [HttpPut("/UpdateTransactionHistory/{id}")]
        public async Task<ActionResult<TransactionHistoryDTO>> UpdateTransactionHistory(int id, [FromBody] TransactionHistoryDTO transactionHistoryDTO)
        {
            await _transactionHistoryService.Update(transactionHistoryDTO);
            return transactionHistoryDTO;
        }

        [HttpDelete("/DeleteTransactionHistory")]
        public async Task<ActionResult<TransactionHistoryDTO>> DeleteTransactioHistory([FromBody] TransactionHistoryDTO transactionHistoryDTO)
        {
            await _transactionHistoryService.Delete(transactionHistoryDTO);
            return transactionHistoryDTO;
        }
    }
}
