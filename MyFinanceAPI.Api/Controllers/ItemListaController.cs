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
    public class ItemListaController : ApiControllerBase
    {
        /// <summary>Não há GET de um item só: a rota devolve os itens da lista (recebe o id da lista).</summary>
        public const string RotaItensDaLista = "GetItemLista";

        private readonly IItemListaService _itemListaService;

        public ItemListaController(IItemListaService itemListaService)
        {
            _itemListaService = itemListaService;
        }

        [HttpGet("/GetItemLista")]
        public async Task<ActionResult<IEnumerable<ItemListaDTO>>> GetItemLista([FromQuery] Paginacao paginacao)
        {
            var userId = UsuarioId;

            var ItemLista = await _itemListaService.ListarAsync(userId);
            return Lista(ItemLista, paginacao);
        }

        [HttpGet("/GetItemListaById/{id}", Name = RotaItensDaLista)]
        public async Task<ActionResult<IEnumerable<ItemListaDTO>>> GetItemListaById(int id, [FromQuery] Paginacao paginacao)
        {
            var userId = UsuarioId;

            var ItemLista = await _itemListaService.GetItemListaByListaId(id, userId);
            return Lista(ItemLista, paginacao);
        }

        /// <summary>201 com o item criado; o Location aponta para os itens da lista dele.</summary>
        [HttpPost("/CreateItemLista")]
        public async Task<ActionResult<ItemListaDTO>> CreateItemLista([FromBody] ItemListaDTO ItemListaDTO)
        {
            var userId = UsuarioId;

            var criado = await _itemListaService.AdicionarAsync(ItemListaDTO, userId);
            return Criado(RotaItensDaLista, criado.ListaId, criado);
        }

        [HttpPut("/UpdateItemLista")]
        public async Task<ActionResult<ItemListaDTO>> UpdateItemLista(ItemListaDTO ItemListaDTO)
        {
            var userId = UsuarioId;

            await _itemListaService.AtualizarAsync(ItemListaDTO, userId);
            return Ok(ItemListaDTO);
        }

        [HttpDelete("/DeleteItemLista/{id}")]
        public async Task<IActionResult> DeleteItemLista(int id)
        {
            var userId = UsuarioId;

            var ItemLista = await _itemListaService.ObterPorIdAsync(id, userId);
            if (ItemLista == null)
                return NaoEncontrado("Item da lista não encontrado.");

            await _itemListaService.RemoverAsync(id, userId);
            return NoContent();
        }
    }
}
