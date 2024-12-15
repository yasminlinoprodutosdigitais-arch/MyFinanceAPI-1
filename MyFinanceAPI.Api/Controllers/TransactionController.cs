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
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("/GetTransactions")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            var update = await _transactionService.GetTransactions();
            if(update is null)
                return NotFound("");
            else
                return Ok(update);
                
        }

        [HttpGet("/GetTransactionByIdCategory/{id}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionByCategory(int idCategory)
        {
            if(idCategory == 0 || idCategory < 0)
                return BadRequest("");

            var transaction = await _transactionService.GetTransactionByCategory(idCategory);

            if(transaction is null)
                return NotFound();
            else
                return Ok(transaction);
        }

        [HttpGet("/GetTransactionById/{id}", Name="GetUpdate")]
        public async Task<ActionResult<Transaction>> GetTransactionById(int id)
        {
            if(id == 0 || id < 0)
                return BadRequest();
            var update = await _transactionService.GetTransactionById(id);

            if(update is null)
                return NotFound();
            else
                return Ok(update);
        }

        [HttpGet("/GetTransactionByDate/{date}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionByDate(DateTime date)
        {
            var transaction = await _transactionService.GetTransactionByDate(date);
            if(transaction is null)
                return NotFound();
            else
                return Ok(transaction);
        }

        [HttpGet("/GetTransactionGroupingByDate/{date}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionGroupingByDate(DateTime date)
        {
            var transaction = await _transactionService.GetTransactionGroupingByDate(date);
            if(transaction is null)
                return NotFound();
            else
                return Ok(transaction);
        }

        [HttpPost("/CreateTransaction")]
        public async Task<ActionResult<TransactionDTO>> CreateTransaction([FromBody] TransactionDTO transactionDTO)
        {
            await _transactionService.Add(transactionDTO);
            return new CreatedAtRouteResult("Getupdate", new {id = transactionDTO.Id}, transactionDTO);
        }

        [HttpPut("/UpdateTransaction")]
        public async Task<ActionResult<TransactionDTO>> UpdateTransaction([FromBody] TransactionDTO transactionDTO)
        {
            await _transactionService.Update(transactionDTO);
            return transactionDTO;
        }

        [HttpDelete("/DeleteTransaction/{id}")]
        public async Task<ActionResult<TransactionDTO>> DeleteAccountHistory(int id)
        {
            var transaction = await _transactionService.GetTransactionById(id);
            await _transactionService.Delete(id);
            return transaction;
        }
    }
}
