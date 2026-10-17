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
    public class AccountsController : ApiControllerBase
    {
        public const string RotaPorId = "GetAccount";

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

        [HttpGet("/GetAccountById/{id}", Name = RotaPorId)]
        public async Task<ActionResult<AccountDTO>> GetAccountById(int id)
        {
            var userId = UsuarioId;

            var account = await _accountService.GetAccountById(id, userId);
            if (account is null)
                return NaoEncontrado("Conta não encontrada.");

            return Ok(account);
        }

        [HttpPost("/CreateAccount")]
        public async Task<ActionResult<AccountDTO>> CreateAccount([FromBody] AccountDTO accountDTO)
        {
            var userId = UsuarioId;

            var criada = await _accountService.Add(accountDTO, userId);
            return Criado(RotaPorId, criada.Id, criada);
        }

        [HttpPut("/UpdateAccount")]
        public async Task<ActionResult<AccountDTO>> UpdateAccount([FromBody] AccountDTO accountDTO)
        {
            var userId = UsuarioId;

            await _accountService.Update(accountDTO, userId);
            return Ok(accountDTO);
        }

        [HttpDelete("/DeleteAccount/{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var userId = UsuarioId;

            var account = await _accountService.GetAccountById(id, userId);
            if (account == null)
                return NaoEncontrado("Conta não encontrada.");

            await _accountService.Remove(id, userId);
            return NoContent();
        }
    }
}
