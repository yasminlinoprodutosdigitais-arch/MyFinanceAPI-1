// MyFinanceAPI.Api/Controllers/ExtratoBancarioItemController.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Application.DTO.Extrato;
using MyFinanceAPI.Application.Interfaces;

namespace MyFinanceAPI.Api.Controllers
{
    [Authorize(Policy = "Admin")]
    public class ExtratoBancarioItemController : ApiControllerBase
    {
        public const string RotaPorId = "GetExtratoBancarioItemById";

        private readonly IExtratoBancarioItemService _extratoBancarioItemService;
        private readonly IAcertosService _acertosService;
        private readonly IResumoCategoriasService _resumoCategorias;
        private readonly INotaFiscalService _notaFiscal;
        private readonly IResumoContasService _resumoContas;

        public ExtratoBancarioItemController(
            IExtratoBancarioItemService extratoBancarioItemService,
            IAcertosService acertosService,
            IResumoCategoriasService resumoCategorias,
            INotaFiscalService notaFiscal,
            IResumoContasService resumoContas)
        {
            _extratoBancarioItemService = extratoBancarioItemService;
            _acertosService = acertosService;
            _resumoCategorias = resumoCategorias;
            _notaFiscal = notaFiscal;
            _resumoContas = resumoContas;
        }

        /// <summary>
        /// Acertos com pessoas (card #45, parte 3): quem me deve, por pessoa. inicio/fim em "AAAA-MM" (vazio = desde o
        /// início / até hoje); bancoId filtra as compras da conta ou do cartão e os pagamentos que as abatem.
        /// </summary>
        /// <summary>Card #76, fase 6: lê a NFC-e do PR pelo link do QR Code. POST porque o link leva a chave da nota.</summary>
        [HttpPost("/LerNotaFiscal")]
        public async Task<ActionResult<MyFinanceAPI.Application.DTO.Movimentacoes.NotaFiscalDTO>> LerNotaFiscal([FromBody] MyFinanceAPI.Application.DTO.Movimentacoes.LerNotaFiscalDTO dto) =>
            Ok(await _notaFiscal.LerAsync(UsuarioId, dto?.Link));

        [HttpPut("/SalvarNotaFiscal/{itemId:int}")]
        public async Task<ActionResult<MyFinanceAPI.Application.DTO.Movimentacoes.NotaSalvaDTO>> SalvarNotaFiscal(int itemId, [FromBody] MyFinanceAPI.Application.DTO.Movimentacoes.NotaFiscalDTO nota) =>
            Ok(await _notaFiscal.SalvarAsync(UsuarioId, itemId, nota));

        [HttpGet("/GetNotaFiscal/{itemId:int}")]
        public async Task<ActionResult<MyFinanceAPI.Application.DTO.Movimentacoes.NotaFiscalDTO>> GetNotaFiscal(int itemId)
        {
            var nota = await _notaFiscal.ObterAsync(UsuarioId, itemId);
            return nota is null ? NaoEncontrado("Este lançamento não tem nota.") : Ok(nota);
        }

        [HttpDelete("/DeleteNotaFiscal/{itemId:int}")]
        public async Task<IActionResult> DeleteNotaFiscal(int itemId)
        {
            await _notaFiscal.RemoverAsync(UsuarioId, itemId);
            return NoContent();
        }

        /// <summary>Card #77: números por conta no mês, os mesmos na listagem e no detalhe.</summary>
        [HttpGet("/GetResumoContas")]
        public async Task<ActionResult<MyFinanceAPI.Application.DTO.Movimentacoes.ResumoContasDTO>> GetResumoContas([FromQuery] string? month) =>
            Ok(await _resumoContas.ObterAsync(UsuarioId, month));

        [HttpGet("/GetResumoCategorias")]
        public async Task<ActionResult<MyFinanceAPI.Application.DTO.Movimentacoes.ResumoCategoriasDTO>> GetResumoCategorias([FromQuery] string? month, [FromQuery] int? bancoId) =>
            Ok(await _resumoCategorias.ObterAsync(UsuarioId, month, bancoId));

        [HttpGet("/GetEvolucaoExtratos")]
        public async Task<ActionResult<MyFinanceAPI.Application.DTO.Movimentacoes.EvolucaoExtratosDTO>> GetEvolucaoExtratos(
            [FromQuery] string? month, [FromQuery] int meses = 6) =>
            Ok(await _resumoCategorias.ObterEvolucaoAsync(UsuarioId, month, meses));

        [HttpGet("/GetAcertos")]
        public async Task<ActionResult<MyFinanceAPI.Application.DTO.Movimentacoes.AcertosDTO>> GetAcertos(
            [FromQuery] string? inicio, [FromQuery] string? fim, [FromQuery] int? bancoId)
        {
            return Ok(await _acertosService.ObterAsync(UsuarioId, inicio, fim, bancoId));
        }

        // GET /GetExtratoBancarioItemById/{id}
        [HttpGet("/GetExtratoBancarioItemById/{id:int}", Name = RotaPorId)]
        public async Task<ActionResult<ExtratoBancarioItemDTO>> GetExtratoItemById(int id)
        {
            var userId = UsuarioId;

            var item = await _extratoBancarioItemService.GetByIdAsync(id, userId);
            if (item == null)
                return NaoEncontrado("Item de extrato não encontrado.");

            return Ok(item);
        }

        // POST /CreateExtratoBancarioItem
        [HttpPost("/CreateExtratoBancarioItem")]
        public async Task<ActionResult<ExtratoBancarioItemDTO>> CreateExtratoItem([FromBody] ExtratoBancarioItemDTO dto)
        {
            var userId = UsuarioId;

            var created = await _extratoBancarioItemService.AddAsync(dto, userId);
            return Criado(RotaPorId, created.Id, created);
        }

        // PUT /UpdateExtratoBancarioItem
        /// <summary>Card #76, fase 3: divide o lançamento nestas partes; lista vazia desfaz a divisão.</summary>
        [HttpPut("/DividirExtratoBancarioItem/{id:int}")]
        public async Task<IActionResult> DividirExtratoItem(int id, [FromBody] List<ExtratoItemParteDTO> partes)
        {
            await _extratoBancarioItemService.DividirAsync(id, partes ?? new List<ExtratoItemParteDTO>(), UsuarioId);
            return NoContent();
        }

        [HttpPut("/UpdateExtratoBancarioItem")]
        public async Task<ActionResult<ExtratoBancarioItemDTO>> UpdateExtratoItem([FromBody] ExtratoBancarioItemDTO dto)
        {
            var userId = UsuarioId;

            if (dto.Id <= 0)
                return ErroRequisicao("Dados inválidos.");

            await _extratoBancarioItemService.UpdateAsync(dto, userId);
            return Ok(dto);
        }

        // DELETE /DeleteExtratoBancarioItem/{id}
        [HttpDelete("/DeleteExtratoBancarioItem/{id:int}")]
        public async Task<IActionResult> DeleteExtratoItem(int id)
        {
            var userId = UsuarioId;

            var item = await _extratoBancarioItemService.GetByIdAsync(id, userId);
            if (item == null)
                return NaoEncontrado("Item de extrato não encontrado.");

            await _extratoBancarioItemService.RemoveAsync(id, userId);
            return NoContent();
        }

        /// <summary>
        /// Retorna os itens de extrato do usuário por mês (month = yyyy-MM).
        /// </summary>
        // GET /GetExtratoBancarioItensByMonth?month=2025-11
        [HttpGet("/GetExtratoBancarioItensByMonth")]
        public async Task<ActionResult<IEnumerable<ExtratoBancarioItemDTO>>> GetByMonth(
            [FromQuery] string month, [FromQuery] int? bancoId, [FromQuery] Paginacao paginacao)
        {
            var userId = UsuarioId;

            if (string.IsNullOrWhiteSpace(month))
                return ErroRequisicao("Parâmetro 'month' é obrigatório no formato yyyy-MM.");

            // Tenta montar um dia 01 desse mês
            if (!DateTime.TryParseExact(
                    month + "-01",
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var dt))
            {
                return ErroRequisicao("Formato de mês inválido. Use yyyy-MM (ex.: 2025-11).");
            }
            var itens = await _extratoBancarioItemService.GetByMonthAsync(userId, dt.Year, dt.Month, bancoId ?? null);
            return Lista(itens, paginacao);
        }

    }
}
