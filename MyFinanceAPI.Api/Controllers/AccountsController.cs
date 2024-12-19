using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("GetAccounts")]
        public async Task<ActionResult<IEnumerable<AccountDTO>>> GetAccounts()
        {

            try
            {
                var accounts = await _accountService.GetAccounts();
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Token expired or invalid.", error = ex.Message });
            }


        }


        [HttpGet("/GetAccountById/{id}", Name = "GetAccount")]
        public async Task<ActionResult<AccountDTO>> GetAccountById(int id)
        {
            var account = await _accountService.GetAccountById(id);
            if (account is null)
                return NotFound("Account Bad Request");
            else
                return Ok(account);
        }

        [HttpGet("/GetAccountsByCategory/{id}")]
        public async Task<ActionResult<ActionResult<AccountDTO>>> GetAccountByCategory(int idCategory)
        {
            if (idCategory == 0 || idCategory < 0)
                return BadRequest("");

            var Accounts = await _accountService.GetAccountByCategory(idCategory);

            if (Accounts is null)
                return NotFound();
            else
                return Ok(Accounts);
        }

        [HttpPost("/CreateAccount")]
        public async Task<ActionResult<AccountDTO>> CreateAccount([FromBody] AccountDTO accountDTO)
        {
            if (accountDTO is null)
                return BadRequest("Invalid Data");

            await _accountService.Add(accountDTO);
            return new CreatedAtRouteResult("GetCategory", new { id = accountDTO.Id }, accountDTO);

        }

        [HttpPut("/UpdateAccount")]
        public async Task<ActionResult<AccountDTO>> UpdateAccount(AccountDTO accountDTO)
        {
            await _accountService.Update(accountDTO);
            return accountDTO;
        }

        [HttpDelete("/DeleteAccount/{id}")]
        public async Task<ActionResult<AccountDTO>> DeleteAccount(int id)
        {
            var account = await _accountService.GetAccountById(id);
            await _accountService.Remove(id);
            return account;
        }
    }
}
