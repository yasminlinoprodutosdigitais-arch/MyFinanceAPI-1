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
    public class ListasController : ApiControllerBase
    {
        public const string RotaPorId = "GetLista";

        private readonly IListaService _ListaService;

        public ListasController(IListaService ListaService)
        {
            _ListaService = ListaService;
        }

        [HttpGet("/GetListas")]
        public async Task<ActionResult<IEnumerable<ListaDTO>>> GetListas()
        {
            var userId = UsuarioId;

            var Listas = await _ListaService.ListarAsync(userId);
            return Ok(Listas);
        }

        [HttpGet("/GetListaById/{id}", Name = RotaPorId)]
        public async Task<ActionResult<ListaDTO>> GetListaById(int id)
        {
            var userId = UsuarioId;

            var Lista = await _ListaService.ObterPorIdAsync(id, userId);
            if (Lista is null)
                return NaoEncontrado("Lista não encontrada.");

            return Ok(Lista);
        }

        [HttpPost("/CreateLista")]
        public async Task<ActionResult<ListaDTO>> CreateLista([FromBody] ListaDTO ListaDTO)
        {
            var userId = UsuarioId;

            var criada = await _ListaService.AdicionarAsync(ListaDTO, userId);
            return Criado(RotaPorId, criada.Id, criada);
        }

        [HttpPut("/UpdateLista")]
        public async Task<ActionResult<ListaDTO>> UpdateLista(ListaDTO ListaDTO)
        {
            var userId = UsuarioId;

            await _ListaService.AtualizarAsync(ListaDTO, userId);
            return Ok(ListaDTO);
        }

        [HttpDelete("/DeleteLista/{id}")]
        public async Task<IActionResult> DeleteLista(int id)
        {
            var userId = UsuarioId;

            var Lista = await _ListaService.ObterPorIdAsync(id, userId);
            if (Lista == null)
                return NaoEncontrado("Lista não encontrada.");

            await _ListaService.RemoverAsync(id, userId);
            return NoContent();
        }
    }
}
