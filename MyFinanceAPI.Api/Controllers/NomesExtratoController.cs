using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;

namespace MyFinanceAPI.Api.Controllers
{
    /// <summary>Card #79 — "Nomes do extrato": classificar o nome que vem no extrato ("CIRCUITO") e juntar parecidos.</summary>
    [Authorize(Policy = "Admin")]
    public class NomesExtratoController(INomesExtratoService nomes) : ApiControllerBase
    {
        [HttpGet("/GetNomesExtrato")]
        public async Task<ActionResult<IEnumerable<NomeExtratoDTO>>> Listar([FromQuery] Paginacao paginacao) =>
            Lista(await nomes.ListarAsync(UsuarioId), paginacao);

        /// <summary>Cria (ou altera) a regra "nome do extrato é igual a" de cada nome e, se pedido, aplica nos já importados.</summary>
        [HttpPost("/ClassificarNomesExtrato")]
        public async Task<ActionResult<ClassificacaoNomesResultadoDTO>> Classificar([FromBody] ClassificarNomesDTO pedido) =>
            Ok(await nomes.ClassificarAsync(UsuarioId, pedido ?? new ClassificarNomesDTO()));

        [HttpGet("/GetGruposParecidos")]
        public async Task<ActionResult<IEnumerable<GrupoParecidosDTO>>> Grupos([FromQuery] Paginacao paginacao) =>
            Lista(await nomes.GruposAsync(UsuarioId), paginacao);

        /// <summary>Deixa uma regra do grupo e apaga as outras; a semelhança passa a cobrir os nomes delas.</summary>
        [HttpPost("/JuntarNomesParecidos")]
        public async Task<ActionResult<ClassificacaoNomesResultadoDTO>> Juntar([FromBody] JuntarNomesDTO pedido) =>
            Ok(await nomes.JuntarAsync(UsuarioId, pedido ?? new JuntarNomesDTO()));
    }
}
