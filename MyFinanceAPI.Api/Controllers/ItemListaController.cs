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
    public class ItemListaController : ControllerBase
    {
        private readonly IItemListaService _ItemListaervice;
        private readonly IUserContextService _userContextService;

        public ItemListaController(IItemListaService ItemListaervice, IUserContextService userContextService)
        {
            _ItemListaervice = ItemListaervice;
            _userContextService = userContextService;
        }

        [HttpGet("/GetItemLista")]
        public async Task<ActionResult<IEnumerable<ItemListaDTO>>> GetItemLista()
        {
            try
            {
                int? userId = _userContextService.GetUserIdFromClaims();
                if (userId == null)
                    return Unauthorized(new { message = "Usuário não autorizado!" });

                var ItemLista = await _ItemListaervice.GetItemLista(userId.Value);
                return Ok(ItemLista);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Token expired or invalid.", error = ex.Message });
            }
        }


        [HttpGet("/GetItemListaById/{id}", Name = "GetItemLista")]
        public async Task<ActionResult<ItemListaDTO>> GetItemListaById(int id)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                var ItemLista = await _ItemListaervice.GetItemListaById(id, userId);
                if (ItemLista is null)
                    return NotFound("ItemLista não encontrada!");
                else
                    return Ok(ItemLista);

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

        [HttpPost("/CreateItemLista")]
        public async Task<ActionResult> CreateItemLista([FromBody] ItemListaDTO ItemListaDTO)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                if (ItemListaDTO is null)
                    return BadRequest("Dados inválidos.");

                await _ItemListaervice.Add(ItemListaDTO, userId);
                return Ok("ItemLista criada com sucesso!");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("/UpdateItemLista")]
        public async Task<ActionResult<ItemListaDTO>> UpdateItemLista(ItemListaDTO ItemListaDTO)
        {
            var userId = _userContextService.GetUserIdFromClaims();
            if (userId == 0)
                return Unauthorized("Usuário não autorizado!");

            await _ItemListaervice.UpdateAsync(ItemListaDTO, userId);
            return ItemListaDTO;
        }

        [HttpDelete("/DeleteItemLista/{id}")]
        public async Task<ActionResult<ItemListaDTO>> DeleteItemLista(int id)
        {
            try 
            {
                var userId = _userContextService.GetUserIdFromClaims();
                    if (userId == 0)
                return Unauthorized("Usuário não autorizado!");

                var ItemLista = await _ItemListaervice.GetItemListaById(id, userId);
                if(ItemLista == null)
                    return NotFound(new {message = "ItemLista não encontrada."});

                await _ItemListaervice.Remove(id, userId);
                return Ok(ItemLista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao deletar ItemLista.", error = ex.Message, detail = ex.StackTrace });
            }
        }
    }
}
