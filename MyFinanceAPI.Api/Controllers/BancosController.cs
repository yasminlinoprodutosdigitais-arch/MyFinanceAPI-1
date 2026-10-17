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
    public class BancosController : ApiControllerBase
    {
        public const string RotaPorId = "GetBanco";

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

        [HttpGet("/GetBancoById/{id}", Name = RotaPorId)]
        public async Task<ActionResult<BancoDTO>> GetBancoById(int id)
        {
            var userId = UsuarioId;

            var Banco = await _BancoService.ObterPorIdAsync(id, userId);
            if (Banco is null)
                return NaoEncontrado("Banco não encontrado.");

            return Ok(Banco);
        }

        [HttpPost("/CreateBanco")]
        public async Task<ActionResult<BancoDTO>> CreateBanco([FromBody] BancoDTO BancoDTO)
        {
            var userId = UsuarioId;

            var criado = await _BancoService.AdicionarAsync(BancoDTO, userId);
            return Criado(RotaPorId, criado.Id, criado);
        }

        [HttpPut("/UpdateBanco")]
        public async Task<ActionResult<BancoDTO>> UpdateBanco(BancoDTO BancoDTO)
        {
            var userId = UsuarioId;

            await _BancoService.AtualizarAsync(BancoDTO, userId);
            return Ok(BancoDTO);
        }

        [HttpDelete("/DeleteBanco/{id}")]
        public async Task<IActionResult> DeleteBanco(int id)
        {
            var userId = UsuarioId;

            var Banco = await _BancoService.ObterPorIdAsync(id, userId);
            if (Banco == null)
                return NaoEncontrado("Banco não encontrado.");

            await _BancoService.RemoverAsync(id, userId);
            return NoContent();
        }
    }
}
