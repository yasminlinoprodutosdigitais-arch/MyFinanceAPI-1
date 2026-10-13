using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Api.Controllers
{
    [Authorize(Policy = "Admin")]
    [Route("api/[controller]")]
    public class AccountsController : ApiControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("/GetAccounts")]
        public async Task<ActionResult<IEnumerable<AccountDTO>>> GetAccounts()
        {
            var userId = UsuarioId;

            var accounts = await _accountService.GetAccounts(userId);
            return Ok(accounts);
        }

        [HttpGet("/GetContasAtivas")]
        public async Task<ActionResult<IEnumerable<AccountDTO>>> GetContasAtivas()
        {
            var userId = UsuarioId;

            var accounts = await _accountService.GetContasAtivas(userId);
            return Ok(accounts);
        }

        [HttpGet("/GetAccountById/{id}", Name = "GetAccount")]
        public async Task<ActionResult<AccountDTO>> GetAccountById(int id)
        {
            var userId = UsuarioId;

            var account = await _accountService.GetAccountById(id, userId);
            if (account is null)
                return NotFound("Account Bad Request");
            else
                return Ok(account);

        }

        [HttpGet("/GetAccountsByCategory/{id}")]
        public async Task<ActionResult<ActionResult<AccountDTO>>> GetAccountByCategory(int id)
        {
            var userId = UsuarioId;

            if (id == 0 || id < 0)
                return BadRequest("");

            var Accounts = await _accountService.GetAccountByCategory(id, userId);

            if (Accounts is null)
                return NotFound();
            else
                return Ok(Accounts);
        }

        [HttpPost("/CreateAccount")]
        public async Task<ActionResult<AccountDTO>> CreateAccount([FromBody] AccountDTO accountDTO)
        {
            var userId = UsuarioId;

            if (accountDTO is null)
                return BadRequest("Dados inválidos.");

            await _accountService.Add(accountDTO, userId);
            return new CreatedAtRouteResult("GetCategory", new { id = accountDTO.Id }, accountDTO);
        }

        [HttpPut("/UpdateAccount")]
        public async Task<ActionResult<AccountDTO>> UpdateAccount([FromBody] AccountDTO accountDTO)
        {
            var userId = UsuarioId;

            await _accountService.Update(accountDTO, userId);
            return accountDTO;

        }

        [HttpDelete("/DeleteAccount/{id}")]
        public async Task<ActionResult<AccountDTO>> DeleteAccount(int id)
        {
            var userId = UsuarioId;

            var account = await _accountService.GetAccountById(id, userId);
            if(account == null)
                return NotFound(new {message = "Conta não encontrada"});

            await _accountService.Remove(id, userId);
            return Ok(account);
        }
    }
}
