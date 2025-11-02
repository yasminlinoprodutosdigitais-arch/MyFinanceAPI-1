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
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IUserContextService _userContextService;

        public AccountsController(IAccountService accountService, IUserContextService userContextService)
        {
            _accountService = accountService;
            _userContextService = userContextService;
        }

        [HttpGet("/GetAccounts")]
        public async Task<ActionResult<IEnumerable<AccountDTO>>> GetAccounts()
        {
            try
            {
                int? userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized(new { message = "User not authorized" });

                var accounts = await _accountService.GetAccounts(userId.Value);
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
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("User not authorized");

                var account = await _accountService.GetAccountById(id, userId);
                if (account is null)
                    return NotFound("Account Bad Request");
                else
                    return Ok(account);

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro inesperado.", details = ex.Message });
            }
        }

        [HttpGet("/GetAccountsByCategory/{id}")]
        public async Task<ActionResult<ActionResult<AccountDTO>>> GetAccountByCategory(int id)
        {
            var userId = _userContextService.GetUserIdFromClaims();
            if (userId == 0)
                return Unauthorized("User not authorized");

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
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                if (accountDTO is null)
                    return BadRequest("Dados inválidos.");

                await _accountService.Add(accountDTO, userId);
                return new CreatedAtRouteResult("GetCategory", new { id = accountDTO.Id }, accountDTO);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("/UpdateAccount")]
        public async Task<ActionResult<AccountDTO>> UpdateAccount(AccountDTO accountDTO)
        {
            var userId = _userContextService.GetUserIdFromClaims();
            if (userId == 0)
                return Unauthorized("User not authorized");

            await _accountService.Update(accountDTO, userId);
            return accountDTO;
        }

        [HttpDelete("/DeleteAccount/{id}")]
        public async Task<ActionResult<AccountDTO>> DeleteAccount(int id)
        {
            try 
            {
                var userId = _userContextService.GetUserIdFromClaims();
                    if (userId == 0)
                return Unauthorized("User not authorized");

                var account = await _accountService.GetAccountById(id, userId);
                if(account == null)
                    return NotFound(new {message = "Conta não encontrada"});

                await _accountService.Remove(id, userId);
                return Ok(account);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao deletar conta.", error = ex.Message, detail = ex.StackTrace });
            }
        }
    }
}
