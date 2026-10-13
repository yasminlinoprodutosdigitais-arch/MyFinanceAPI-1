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
    public class BancosController : ApiControllerBase
    {
        private readonly IBancoService _BancoService;

        public BancosController(IBancoService BancoService)
        {
            _BancoService = BancoService;
        }

        [HttpGet("/GetBancos")]
        public async Task<ActionResult<IEnumerable<BancoDTO>>> GetBancos()
        {
            var userId = UsuarioId;

            var Bancos = await _BancoService.ListarAsync(userId);
            return Ok(Bancos);
        }

        [HttpGet("/GetBancoById/{id}", Name = "GetBanco")]
        public async Task<ActionResult<BancoDTO>> GetBancoById(int id)
        {
            var userId = UsuarioId;

            var Banco = await _BancoService.ObterPorIdAsync(id, userId);
            if (Banco is null)
                return NotFound("Banco Bad Request");
            else
                return Ok(Banco);

        }

        [HttpPost("/CreateBanco")]
        public async Task<ActionResult> CreateBanco([FromBody] BancoDTO BancoDTO)
        {
            var userId = UsuarioId;

            if (BancoDTO is null)
                return BadRequest("Dados inválidos.");

            await _BancoService.AdicionarAsync(BancoDTO, userId);
            
            return StatusCode(StatusCodes.Status201Created, new { message = "Banco criado com sucesso!" });
        }

        [HttpPut("/UpdateBanco")]
        public async Task<ActionResult<BancoDTO>> UpdateBanco(BancoDTO BancoDTO)
        {
            var userId = UsuarioId;

            if (BancoDTO is null)
                return BadRequest("Dados inválidos.");

            await _BancoService.AtualizarAsync(BancoDTO, userId);
            return BancoDTO;
        }

        [HttpDelete("/DeleteBanco/{id}")]
        public async Task<ActionResult<BancoDTO>> DeleteBanco(int id)
        {
            var userId = UsuarioId;

            var Banco = await _BancoService.ObterPorIdAsync(id, userId);
            if (Banco == null)
                return NotFound(new { message = "Banco não encontrado." });

            await _BancoService.RemoverAsync(id, userId);
            return Ok(Banco);
        }
    }
}
