using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Api.Autorizacao;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;

namespace MyFinanceAPI.Api.Controllers
{
    /// <summary>Regras de classificação (card #76, fase 1): "a descrição contém X" → categoria, tipo e pessoa.</summary>
    [Authorize(Policy = Politicas.Usuario)]
    public class RegrasClassificacaoController(IRegraClassificacaoService regras, IClassificacaoService classificacao) : ApiControllerBase
    {
        public const string RotaPorId = "GetRegraClassificacaoById";

        [HttpGet("/GetRegrasClassificacao")]
        public async Task<ActionResult<IEnumerable<RegraClassificacaoDTO>>> Listar([FromQuery] Paginacao paginacao) =>
            Lista(await regras.ListarAsync(UsuarioId), paginacao);

        [HttpGet("/GetRegraClassificacaoById/{id:int}", Name = RotaPorId)]
        public async Task<ActionResult<RegraClassificacaoDTO>> ObterPorId(int id)
        {
            var regra = await regras.ObterPorIdAsync(id, UsuarioId);
            return regra is null ? NaoEncontrado("Regra não encontrada.") : Ok(regra);
        }

        [HttpPost("/CreateRegraClassificacao")]
        public async Task<ActionResult<RegraClassificacaoDTO>> Criar([FromBody] RegraClassificacaoDTO dto)
        {
            var criada = await regras.AdicionarAsync(dto, UsuarioId);
            return Criado(RotaPorId, criada.Id, criada);
        }

        [HttpPut("/UpdateRegraClassificacao")]
        public async Task<ActionResult<RegraClassificacaoDTO>> Atualizar([FromBody] RegraClassificacaoDTO dto)
        {
            if (dto.Id <= 0)
                return ErroRequisicao("Dados inválidos.");

            await regras.AtualizarAsync(dto, UsuarioId);
            return Ok(dto);
        }

        /// <summary>Prévia ao vivo de uma regra ainda não gravada: quantos batem, sem categoria, conflitos e exemplos.</summary>
        [HttpPost("/GetPreviaRegraClassificacao")]
        public async Task<ActionResult<PreviaRegraResultadoDTO>> Previa([FromBody] PreviaRegraDTO previa) =>
            Ok(await classificacao.PreviaAsync(UsuarioId, previa ?? new PreviaRegraDTO()));

        /// <summary>Aplica a regra nos lançamentos sem categoria que batem com ela (depois de a pessoa confirmar).</summary>
        [HttpPost("/AplicarRegraClassificacao/{id:int}")]
        public async Task<ActionResult<AplicacaoRegraDTO>> Aplicar(int id) =>
            Ok(await classificacao.AplicarAsync(UsuarioId, id));

        /// <summary>Nova ordem de prioridade (o primeiro id fica no topo: "vale a de cima").</summary>
        [HttpPut("/ReordenarRegrasClassificacao")]
        public async Task<IActionResult> Reordenar([FromBody] List<int> ids)
        {
            await regras.ReordenarAsync(UsuarioId, ids ?? []);
            return NoContent();
        }

        /// <summary>"Sugestões do seu extrato": textos repetidos em lançamentos sem categoria que ainda não têm regra.</summary>
        [HttpGet("/GetSugestoesRegras")]
        public async Task<ActionResult<IEnumerable<SugestaoRegraDTO>>> SugestoesDeRegra([FromQuery] Paginacao paginacao) =>
            Lista(await classificacao.SugestoesDeRegraAsync(UsuarioId), paginacao);

        /// <summary>Sugestão de categoria pelo histórico para os lançamentos sem categoria pedidos.</summary>
        [HttpPost("/GetSugestoesCategoria")]
        public async Task<ActionResult<IReadOnlyList<SugestaoCategoriaDTO>>> Sugestoes([FromBody] List<int> itemIds) =>
            Ok(await classificacao.SugestoesAsync(UsuarioId, itemIds ?? []));

        [HttpDelete("/DeleteRegraClassificacao/{id:int}")]
        public async Task<IActionResult> Excluir(int id)
        {
            await regras.RemoverAsync(id, UsuarioId);
            return NoContent();
        }
    }
}
