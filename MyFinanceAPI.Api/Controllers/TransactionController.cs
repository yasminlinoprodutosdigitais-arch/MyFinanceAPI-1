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
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IUserContextService _userContextService;

        public TransactionController(ITransactionService transactionService, IUserContextService userContextService)
        {
            _transactionService = transactionService;
            _userContextService = userContextService;
        }

        [HttpGet("/GetTransactions")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            var userId = _userContextService.GetUserIdFromClaims();
            if (userId == 0)
                return Unauthorized("User not authorized");

            var update = await _transactionService.GetTransactions(userId);
            if (update is null)
                return NotFound("");
            else
                return Ok(update);

        }

        [HttpGet("/GetTransactionById/{id}", Name = "GetUpdate")]
        public async Task<ActionResult<Transaction>> GetTransactionById(int id)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized(new { message = "User not authorized" });

                if (id <= 0)
                    return BadRequest(new { message = "Invalid transaction ID." });

                var transaction = await _transactionService.GetTransactionById(id, userId);

                if (transaction == null)
                    return NotFound(new { message = "Transaction not found." });

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpGet("/GetTransactionByDate/{date}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionByDate(DateTime date)
        {
            var userId = _userContextService.GetUserIdFromClaims();
            if (userId == 0)
                return Unauthorized("User not authorized");

            var transaction = await _transactionService.GetTransactionByDate(date, userId);
            if (transaction is null)
                return NotFound();
            else
                return Ok(transaction);
        }

        [HttpGet("/GetTransactionGroupingByDate/{date}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionGroupingByDate(DateTime date)
        {
            var userId = _userContextService.GetUserIdFromClaims();
            if (userId == 0)
                return Unauthorized("User not authorized");

            var transaction = await _transactionService.GetTransactionGroupingByDate(date, userId);
            if (transaction is null)
                return NotFound();
            else
                return Ok(transaction);
        }

        [HttpPost("/CreateTransaction")]
        public async Task<ActionResult<TransactionDTO>> CreateTransaction([FromBody] TransactionDTO transactionDTO)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("User not authorized");

                await _transactionService.Add(transactionDTO, userId);
                return new CreatedAtRouteResult("Getupdate", new { id = transactionDTO.Id }, transactionDTO);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred", details = ex.Message });
            }
        }

        [HttpPut("/UpdateTransaction")]
        public async Task<ActionResult<TransactionDTO>> UpdateTransaction([FromBody] TransactionDTO transactionDTO)
        {
            var userId = _userContextService.GetUserIdFromClaims();
            if (userId == 0)
                return Unauthorized("User not authorized");

            await _transactionService.Update(transactionDTO, userId);
            return transactionDTO;
        }

        [HttpDelete("/DeleteTransaction/{id}")]
        public async Task<ActionResult<TransactionDTO>> DeleteAccountHistory(int id)
        {
            var userId = _userContextService.GetUserIdFromClaims();
            if (userId == 0)
                return Unauthorized("User not authorized");

            var transaction = await _transactionService.GetTransactionById(id, userId);
            await _transactionService.Delete(id, userId);
            return transaction;
        }
    }
}
