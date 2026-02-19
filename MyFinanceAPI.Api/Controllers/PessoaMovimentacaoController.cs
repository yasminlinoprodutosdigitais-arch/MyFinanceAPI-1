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
    public class PessoaMovimentacaoController : ControllerBase
    {
        private readonly IPessoaMovimentacaoService _PessoaMovimentacaoService;
        private readonly IUserContextService _userContextService;

        public PessoaMovimentacaoController(IPessoaMovimentacaoService PessoaMovimentacaoService, IUserContextService userContextService)
        {
            _PessoaMovimentacaoService = PessoaMovimentacaoService;
            _userContextService = userContextService;
        }

        [HttpGet("/GetPessoaMovimentacao")]
        public async Task<ActionResult<IEnumerable<PessoaMovimentacaoDTO>>> GetPessoaMovimentacao()
        {
            try
            {
                int? userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized(new { message = "Usuário não autorizado!" });

                var PessoaMovimentacao = await _PessoaMovimentacaoService.GetPessoaMovimentacao(userId.Value);
                return Ok(PessoaMovimentacao);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Token expired or invalid.", error = ex.Message });
            }
        }


        [HttpGet("/GetPessoaMovimentacaoById/{id}", Name = "GetPessoaMovimentacao")]
        public async Task<ActionResult<PessoaMovimentacaoDTO>> GetPessoaMovimentacaoById(int id)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                var PessoaMovimentacao = await _PessoaMovimentacaoService.GetPessoaMovimentacaoById(id, userId);
                if (PessoaMovimentacao is null)
                    return NotFound("PessoaMovimentacao não encontrada!");
                else
                    return Ok(PessoaMovimentacao);

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

        [HttpPost("/CreatePessoaMovimentacao")]
        public async Task<ActionResult> CreatePessoaMovimentacao([FromBody] PessoaMovimentacaoDTO dto)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized(new { message = "Usuário não autorizado!" });

                if (dto is null)
                    return BadRequest(new { message = "Dados inválidos." });

                await _PessoaMovimentacaoService.Add(dto, userId);

                return Ok(new { success = true, message = "PessoaMovimentacao criada com sucesso!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("/UpdatePessoaMovimentacao")]
        public async Task<ActionResult<PessoaMovimentacaoDTO>> UpdatePessoaMovimentacao(PessoaMovimentacaoDTO PessoaMovimentacaoDTO)
        {
            try
            {

                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                await _PessoaMovimentacaoService.UpdateAsync(PessoaMovimentacaoDTO, userId);
                return PessoaMovimentacaoDTO;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atualizar PessoaMovimentacao.", error = ex.Message });
            }
        }

        [HttpDelete("/DeletePessoaMovimentacao/{id}")]
        public async Task<ActionResult<PessoaMovimentacaoDTO>> DeletePessoaMovimentacao(int id)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                var PessoaMovimentacao = await _PessoaMovimentacaoService.GetPessoaMovimentacaoById(id, userId);
                if (PessoaMovimentacao == null)
                    return NotFound(new { message = "PessoaMovimentacao não encontrada." });

                await _PessoaMovimentacaoService.Remove(id, userId);
                return Ok(PessoaMovimentacao);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao deletar PessoaMovimentacao.", error = ex.Message, detail = ex.StackTrace });
            }
        }
    }
}
