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
    public class PessoaMovimentacaoController : ApiControllerBase
    {
        public const string RotaPorId = "GetPessoaMovimentacao";

        private readonly IPessoaMovimentacaoService _PessoaMovimentacaoService;
        private readonly ILimpezaPessoasService _limpeza;

        public PessoaMovimentacaoController(IPessoaMovimentacaoService PessoaMovimentacaoService, ILimpezaPessoasService limpeza)
        {
            _PessoaMovimentacaoService = PessoaMovimentacaoService;
            _limpeza = limpeza;
        }

        /// <summary>Card #76, fase 2: pessoas que parecem estabelecimento (candidatas à limpeza), com quantos lançamentos.</summary>
        [HttpGet("/GetPessoasEstabelecimento")]
        public async Task<ActionResult<IEnumerable<PessoaEstabelecimentoDTO>>> GetPessoasEstabelecimento([FromQuery] Paginacao paginacao) =>
            Lista(await _limpeza.CandidatasAsync(UsuarioId), paginacao);

        /// <summary>Card #76, fase 2: apaga as escolhidas que ainda são candidatas; os lançamentos ficam com o nome.</summary>
        [HttpPost("/RemoverPessoasEstabelecimento")]
        public async Task<ActionResult<PessoasRemovidasDTO>> RemoverPessoasEstabelecimento([FromBody] List<int> ids) =>
            Ok(new PessoasRemovidasDTO { Quantidade = await _limpeza.RemoverAsync(UsuarioId, ids ?? []) });

        [HttpGet("/GetPessoaMovimentacao")]
        public async Task<ActionResult<IEnumerable<PessoaMovimentacaoDTO>>> GetPessoaMovimentacao([FromQuery] Paginacao paginacao)
        {
            var userId = UsuarioId;

            var PessoaMovimentacao = await _PessoaMovimentacaoService.ListarAsync(userId);
            return Lista(PessoaMovimentacao, paginacao);
        }

        [HttpGet("/GetPessoaMovimentacaoById/{id}", Name = RotaPorId)]
        public async Task<ActionResult<PessoaMovimentacaoDTO>> GetPessoaMovimentacaoById(int id)
        {
            var userId = UsuarioId;

            var PessoaMovimentacao = await _PessoaMovimentacaoService.ObterPorIdAsync(id, userId);
            if (PessoaMovimentacao is null)
                return NaoEncontrado("Pessoa não encontrada.");

            return Ok(PessoaMovimentacao);
        }

        [HttpPost("/CreatePessoaMovimentacao")]
        public async Task<ActionResult<PessoaMovimentacaoDTO>> CreatePessoaMovimentacao([FromBody] PessoaMovimentacaoDTO dto)
        {
            var userId = UsuarioId;

            var criada = await _PessoaMovimentacaoService.AdicionarAsync(dto, userId);
            return Criado(RotaPorId, criada.Id, criada);
        }

        [HttpPut("/UpdatePessoaMovimentacao")]
        public async Task<ActionResult<PessoaMovimentacaoDTO>> UpdatePessoaMovimentacao(PessoaMovimentacaoDTO PessoaMovimentacaoDTO)
        {
            var userId = UsuarioId;

            await _PessoaMovimentacaoService.AtualizarAsync(PessoaMovimentacaoDTO, userId);
            return Ok(PessoaMovimentacaoDTO);
        }

        [HttpDelete("/DeletePessoaMovimentacao/{id}")]
        public async Task<IActionResult> DeletePessoaMovimentacao(int id)
        {
            var userId = UsuarioId;

            var PessoaMovimentacao = await _PessoaMovimentacaoService.ObterPorIdAsync(id, userId);
            if (PessoaMovimentacao == null)
                return NaoEncontrado("Pessoa não encontrada.");

            await _PessoaMovimentacaoService.RemoverAsync(id, userId);
            return NoContent();
        }
    }
}
