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
    public class ListasController : ApiControllerBase
    {
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

        [HttpGet("/GetListaById/{id}", Name = "GetLista")]
        public async Task<ActionResult<ListaDTO>> GetListaById(int id)
        {
            var userId = UsuarioId;

            var Lista = await _ListaService.ObterPorIdAsync(id, userId);
            if (Lista is null)
                return NotFound("Lista não encontrada!");
            else
                return Ok(Lista);

        }

        [HttpPost("/CreateLista")]
        public async Task<ActionResult> CreateLista([FromBody] ListaDTO ListaDTO)
        {
            var userId = UsuarioId;

            if (ListaDTO is null)
                return BadRequest("Dados inválidos.");

            await _ListaService.AdicionarAsync(ListaDTO, userId);
            return Ok("Lista criada com sucesso!");
        }

        [HttpPut("/UpdateLista")]
        public async Task<ActionResult<ListaDTO>> UpdateLista(ListaDTO ListaDTO)
        {
            var userId = UsuarioId;

            await _ListaService.AtualizarAsync(ListaDTO, userId);
            return ListaDTO;
        }

        [HttpDelete("/DeleteLista/{id}")]
        public async Task<ActionResult<ListaDTO>> DeleteLista(int id)
        {
            var userId = UsuarioId;

            var Lista = await _ListaService.ObterPorIdAsync(id, userId);
            if(Lista == null)
                return NotFound(new {message = "Lista não encontrada."});

            await _ListaService.RemoverAsync(id, userId);
            return Ok(Lista);
        }
    }
}
