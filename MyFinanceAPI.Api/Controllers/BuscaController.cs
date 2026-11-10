using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;

namespace MyFinanceAPI.Api.Controllers;

/// <summary>Busca rápida do menu (card #72).</summary>
[Authorize(Policy = "Admin")]
public class BuscaController(IBuscaService busca) : ApiControllerBase
{
    /// <summary>Planejamentos, itens, categorias e contas fixas do usuário; menos de 2 letras devolve lista vazia.</summary>
    [HttpGet("/Buscar")]
    public async Task<ActionResult<IEnumerable<BuscaResultadoDTO>>> Buscar([FromQuery] string? q, [FromQuery] Paginacao paginacao) =>
        Lista(await busca.BuscarAsync(UsuarioId, q), paginacao);
}
