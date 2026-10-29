using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;

namespace MyFinanceAPI.Api.Controllers
{
    /// <summary>Listas com campos configuráveis (card #67). Arquivar é a forma de guardar; excluir apaga a lista e os itens.</summary>
    [Authorize(Policy = "Admin")]
    public class ListasController(IListaService listaService) : ApiControllerBase
    {
        public const string RotaPorId = "GetLista";

        /// <summary>Com o resumo de cada lista. <paramref name="arquivadas"/> = pasta Arquivadas (padrão: as em uso).</summary>
        [HttpGet("/listas")]
        public async Task<ActionResult<IEnumerable<ListaDTO>>> GetListas([FromQuery] bool arquivadas, [FromQuery] Paginacao paginacao) =>
            Lista(await listaService.ListarAsync(UsuarioId, arquivadas), paginacao);

        [HttpGet("/listas/{id}", Name = RotaPorId)]
        public async Task<ActionResult<ListaDTO>> GetLista(int id)
        {
            var lista = await listaService.ObterPorIdAsync(id, UsuarioId);
            return lista is null ? NaoEncontrado("Lista não encontrada.") : Ok(lista);
        }

        [HttpPost("/listas")]
        public async Task<ActionResult<ListaDTO>> CreateLista([FromBody] ListaDTO dto)
        {
            var criada = await listaService.AdicionarAsync(dto, UsuarioId);
            return Criado(RotaPorId, criada.Id, criada);
        }

        [HttpPut("/listas/{id}")]
        public async Task<ActionResult<ListaDTO>> UpdateLista(int id, [FromBody] ListaDTO dto)
        {
            dto.Id = id;
            await listaService.AtualizarAsync(dto, UsuarioId);
            return Ok(dto);
        }

        [HttpDelete("/listas/{id}")]
        public async Task<IActionResult> DeleteLista(int id)
        {
            if (await listaService.ObterPorIdAsync(id, UsuarioId) is null)
                return NaoEncontrado("Lista não encontrada.");

            await listaService.RemoverAsync(id, UsuarioId);
            return NoContent();
        }

        [HttpPost("/listas/{id}/arquivar")]
        public async Task<IActionResult> Arquivar(int id)
        {
            await listaService.ArquivarAsync(id, UsuarioId);
            return NoContent();
        }

        [HttpPost("/listas/{id}/desarquivar")]
        public async Task<IActionResult> Desarquivar(int id)
        {
            await listaService.DesarquivarAsync(id, UsuarioId);
            return NoContent();
        }

        /// <summary>201 com a cópia: mesma configuração e série, itens desmarcados, fora da pasta Arquivadas.</summary>
        [HttpPost("/listas/{id}/duplicar")]
        public async Task<ActionResult<ListaDTO>> Duplicar(int id, [FromBody] DuplicarListaDTO? dto)
        {
            var copia = await listaService.DuplicarAsync(id, dto ?? new DuplicarListaDTO(), UsuarioId);
            return Criado(RotaPorId, copia.Id, copia);
        }

        /// <summary>Listas da mesma série (arquivadas inclusive), em ordem de criação: os seletores da comparação.</summary>
        [HttpGet("/listas/{id}/serie")]
        public async Task<ActionResult<IEnumerable<ListaDTO>>> GetSerie(int id, [FromQuery] Paginacao paginacao) =>
            Lista(await listaService.ListarDaSerieAsync(id, UsuarioId), paginacao);

        /// <summary>
        /// Compara esta lista (atual) com <paramref name="com"/> (anterior; sem ela, a criada logo antes na série).
        /// <paramref name="base_"/>: <c>estimado</c> (todos os itens) ou <c>ok</c> (só os feitos).
        /// </summary>
        [HttpGet("/listas/{id}/comparar")]
        public async Task<ActionResult<ComparacaoListasDTO>> Comparar(int id, [FromQuery] int? com, [FromQuery(Name = "base")] string? base_)
        {
            var b = string.IsNullOrWhiteSpace(base_) ? "estimado" : base_.Trim().ToLowerInvariant();
            if (b is not ("estimado" or "ok"))
                return ErroRequisicao("base aceita só: estimado, ok.");

            return Ok(await listaService.CompararAsync(id, com, soFeitos: b == "ok", UsuarioId));
        }

        /// <summary>Liga a lista à série de outra; <c>comListaId: null</c> = sai para uma série própria.</summary>
        [HttpPut("/listas/{id}/serie")]
        public async Task<IActionResult> DefinirSerie(int id, [FromBody] SerieListaDTO dto)
        {
            await listaService.DefinirSerieAsync(id, dto.ComListaId, UsuarioId);
            return NoContent();
        }
    }
}
