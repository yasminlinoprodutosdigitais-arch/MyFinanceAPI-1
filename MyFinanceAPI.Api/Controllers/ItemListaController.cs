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
    public class ItemListaController : ApiControllerBase
    {
        private readonly IItemListaService _itemListaService;

        public ItemListaController(IItemListaService ItemListaervice)
        {
            _itemListaService = ItemListaervice;
        }

        [HttpGet("/GetItemLista")]
        public async Task<ActionResult<IEnumerable<ItemListaDTO>>> GetItemLista()
        {
            var userId = UsuarioId;

            var ItemLista = await _itemListaService.ListarAsync(userId);
            return Ok(ItemLista);
        }

        [HttpGet("/GetItemListaById/{id}", Name = "GetItemLista")]
        public async Task<ActionResult<List<ItemListaDTO>>> GetItemListaById(int id)
        {
            var userId = UsuarioId;

            var ItemLista = await _itemListaService.GetItemListaByListaId(id, userId);
            if (ItemLista is null)
                return NotFound("ItemLista não encontrada!");
            else
                return Ok(ItemLista);

        }

        [HttpPost("/CreateItemLista")]
        public async Task<ActionResult> CreateItemLista([FromBody] ItemListaDTO ItemListaDTO)
        {
            var userId = UsuarioId;

            if (ItemListaDTO is null)
                return BadRequest("Dados inválidos.");

            await _itemListaService.AdicionarAsync(ItemListaDTO, userId);
            return Ok("ItemLista criada com sucesso!");
        }

        [HttpPut("/UpdateItemLista")]
        public async Task<ActionResult<ItemListaDTO>> UpdateItemLista(ItemListaDTO ItemListaDTO)
        {
            var userId = UsuarioId;

            await _itemListaService.AtualizarAsync(ItemListaDTO, userId);
            return ItemListaDTO;
        }

        [HttpDelete("/DeleteItemLista/{id}")]
        public async Task<ActionResult<ItemListaDTO>> DeleteItemLista(int id)
        {
            var userId = UsuarioId;

            var ItemLista = await _itemListaService.ObterPorIdAsync(id, userId);
            if(ItemLista == null)
                return NotFound(new {message = "ItemLista não encontrada."});

            await _itemListaService.RemoverAsync(id, userId);
            return Ok(ItemLista);
        }
    }
}
