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

        public ExtratoBancarioItemController(
            IExtratoBancarioItemService extratoBancarioItemService)
        {
            _extratoBancarioItemService = extratoBancarioItemService;
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
