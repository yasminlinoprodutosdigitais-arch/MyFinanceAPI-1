using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;

namespace MyFinanceAPI.Api.Controllers
{
    /// <summary>
    /// Itens de uma lista (card #67), sempre aninhados em /listas/{id}. As ações em lote são uma operação
    /// só e respondem <c>{ afetados }</c>.
    /// </summary>
    [Authorize(Policy = "Admin")]
    public class ItemListaController(IItemListaService itemListaService) : ApiControllerBase
    {
        /// <summary>Não há GET de um item só: o Location do 201 aponta para os itens da lista.</summary>
        public const string RotaItensDaLista = "GetItensDaLista";

        /// <summary>
        /// Sem período: todos os itens (sem paginação, a menos que pedida), como a tela da lista mostra.
        /// Com <paramref name="de"/> e <paramref name="ate"/> (AAAA-MM-DD, até 366 dias): as ocorrências da agenda
        /// no período (<see cref="OcorrenciaDTO"/>), repetições expandidas.
        /// </summary>
        [HttpGet("/listas/{id}/itens", Name = RotaItensDaLista)]
        public async Task<IActionResult> GetItens(int id, [FromQuery] DateOnly? de, [FromQuery] DateOnly? ate, [FromQuery] Paginacao paginacao)
        {
            if (de is null && ate is null)
                return Lista(await itemListaService.ListarDaListaAsync(id, UsuarioId), paginacao).Result!;
            if (de is null || ate is null)
                return ErroRequisicao("Informe de e ate juntos.");
            if (ate < de || ate.Value.DayNumber - de.Value.DayNumber + 1 > 366)
                return ErroRequisicao("O período vai de de até ate, com no máximo 366 dias.");

            return Lista(await itemListaService.ListarPeriodoAsync(id, de.Value, ate.Value, UsuarioId), paginacao).Result!;
        }

        [HttpPost("/listas/{id}/itens")]
        public async Task<ActionResult<ItemListaDTO>> CreateItem(int id, [FromBody] ItemListaDTO dto)
        {
            var criado = await itemListaService.AdicionarNaListaAsync(id, dto, UsuarioId);
            return Criado(RotaItensDaLista, id, criado);
        }

        [HttpPut("/listas/{id}/itens/{itemId:int}")]
        public async Task<ActionResult<ItemListaDTO>> UpdateItem(int id, int itemId, [FromBody] ItemListaDTO dto)
        {
            await itemListaService.AtualizarNaListaAsync(id, itemId, dto, UsuarioId);
            dto.Id = itemId;
            dto.ListaId = id;
            return Ok(dto);
        }

        [HttpDelete("/listas/{id}/itens/{itemId:int}")]
        public async Task<IActionResult> DeleteItem(int id, int itemId)
        {
            await itemListaService.RemoverDaListaAsync(id, itemId, UsuarioId);
            return NoContent();
        }

        /// <summary>"Só esta" numa ocorrência de item que repete: feito, outro dia ou hora, ou pulada.</summary>
        [HttpPut("/listas/{id}/itens/{itemId:int}/ocorrencias/{dataOriginal}")]
        public async Task<IActionResult> AtualizarOcorrencia(int id, int itemId, DateOnly dataOriginal, [FromBody] AlterarOcorrenciaDTO dto)
        {
            await itemListaService.AtualizarOcorrenciaAsync(id, itemId, dataOriginal, dto, UsuarioId);
            return NoContent();
        }

        /// <summary>"Esta e as próximas": 201 com o item que vale de <c>aPartirDe</c> em diante (ou 200, se for o mesmo item).</summary>
        [HttpPost("/listas/{id}/itens/{itemId:int}/dividir")]
        public async Task<ActionResult<ItemListaDTO>> Dividir(int id, int itemId, [FromBody] DividirItemDTO dto)
        {
            var item = await itemListaService.DividirAsync(id, itemId, dto.APartirDe, dto.Item, UsuarioId);
            return item.Id == itemId ? Ok(item) : Criado(RotaItensDaLista, id, item);
        }

        /// <summary>Exclui esta e as próximas ocorrências (desde a primeira: o item inteiro).</summary>
        [HttpDelete("/listas/{id}/itens/{itemId:int}/ocorrencias")]
        public async Task<IActionResult> Encerrar(int id, int itemId, [FromQuery] DateOnly aPartirDe)
        {
            await itemListaService.EncerrarAsync(id, itemId, aPartirDe, UsuarioId);
            return NoContent();
        }

        [HttpPatch("/listas/{id}/itens/feito")]
        public async Task<ActionResult<ResultadoLoteDTO>> MarcarFeito(int id, [FromBody] MarcarItensDTO dto) =>
            Ok(new ResultadoLoteDTO { Afetados = await itemListaService.MarcarFeitoAsync(id, dto.Ids, dto.Feito, UsuarioId) });

        [HttpPost("/listas/{id}/itens/desmarcar-todos")]
        public async Task<ActionResult<ResultadoLoteDTO>> DesmarcarTodos(int id) =>
            Ok(new ResultadoLoteDTO { Afetados = await itemListaService.DesmarcarTodosAsync(id, UsuarioId) });

        [HttpDelete("/listas/{id}/itens/concluidos")]
        public async Task<ActionResult<ResultadoLoteDTO>> ExcluirConcluidos(int id) =>
            Ok(new ResultadoLoteDTO { Afetados = await itemListaService.ExcluirConcluidosAsync(id, UsuarioId) });
    }
}
