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
    public class ListasController : ControllerBase
    {
        private readonly IListaService _ListaService;
        private readonly IUserContextService _userContextService;

        public ListasController(IListaService ListaService, IUserContextService userContextService)
        {
            _ListaService = ListaService;
            _userContextService = userContextService;
        }

        [HttpGet("/GetListas")]
        public async Task<ActionResult<IEnumerable<ListaDTO>>> GetListas()
        {
            try
            {
                int? userId = _userContextService.GetUserIdFromClaims();
                if (userId == null)
                    return Unauthorized(new { message = "Usuário não autorizado!" });

                var Listas = await _ListaService.GetListas(userId.Value);
                return Ok(Listas);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Token expired or invalid.", error = ex.Message });
            }
        }


        [HttpGet("/GetListaById/{id}", Name = "GetLista")]
        public async Task<ActionResult<ListaDTO>> GetListaById(int id)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == null)
                    return Unauthorized("Usuário não autorizado!");

                var Lista = await _ListaService.GetListaById(id, userId);
                if (Lista is null)
                    return NotFound("Lista não encontrada!");
                else
                    return Ok(Lista);

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

        [HttpPost("/CreateLista")]
        public async Task<ActionResult> CreateLista([FromBody] ListaDTO ListaDTO)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == null)
                    return Unauthorized("Usuário não autorizado!");

                if (ListaDTO is null)
                    return BadRequest("Dados inválidos.");

                await _ListaService.Add(ListaDTO, userId);
                return Ok("Lista criada com sucesso!");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("/UpdateLista")]
        public async Task<ActionResult<ListaDTO>> UpdateLista(ListaDTO ListaDTO)
        {
            var userId = _userContextService.GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized("Usuário não autorizado!");

            await _ListaService.Update(ListaDTO, userId);
            return ListaDTO;
        }

        [HttpDelete("/DeleteLista/{id}")]
        public async Task<ActionResult<ListaDTO>> DeleteLista(int id)
        {
            try 
            {
                var userId = _userContextService.GetUserIdFromClaims();
                    if (userId == null)
                return Unauthorized("Usuário não autorizado!");

                var Lista = await _ListaService.GetListaById(id, userId);
                if(Lista == null)
                    return NotFound(new {message = "Lista não encontrada."});

                await _ListaService.Remove(id, userId);
                return Ok(Lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao deletar Lista.", error = ex.Message, detail = ex.StackTrace });
            }
        }
    }
}
