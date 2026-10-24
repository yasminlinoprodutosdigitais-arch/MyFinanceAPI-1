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
    public class ContasController : ApiControllerBase
    {
        public const string RotaPorId = "GetAccount";

        private readonly IContaService _accountService;

        public ContasController(IContaService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("/GetAccounts")]
        public async Task<ActionResult<IEnumerable<ContaDTO>>> GetAccounts([FromQuery] Paginacao paginacao)
        {
            var userId = UsuarioId;

            var accounts = await _accountService.GetAccounts(userId);
            return Lista(accounts, paginacao);
        }

        [HttpGet("/GetContasAtivas")]
        public async Task<ActionResult<IEnumerable<ContaDTO>>> GetContasAtivas([FromQuery] Paginacao paginacao)
        {
            var userId = UsuarioId;

            var accounts = await _accountService.GetContasAtivas(userId);
            return Lista(accounts, paginacao);
        }

        [HttpGet("/GetAccountById/{id}", Name = RotaPorId)]
        public async Task<ActionResult<ContaDTO>> GetAccountById(int id)
        {
            var userId = UsuarioId;

            var account = await _accountService.GetAccountById(id, userId);
            if (account is null)
                return NaoEncontrado("Conta não encontrada.");

            return Ok(account);
        }

        [HttpPost("/CreateAccount")]
        public async Task<ActionResult<ContaDTO>> CreateAccount([FromBody] ContaDTO accountDTO)
        {
            var userId = UsuarioId;

            var criada = await _accountService.Add(accountDTO, userId);
            return Criado(RotaPorId, criada.Id, criada);
        }

        [HttpPut("/UpdateAccount")]
        public async Task<ActionResult<ContaDTO>> UpdateAccount([FromBody] ContaDTO accountDTO)
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
