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
    [Route("api/[controller]")]
    public class ExtratoBancarioItemController : ApiControllerBase
    {
        private readonly IExtratoBancarioItemService _extratoBancarioItemService;

        public ExtratoBancarioItemController(
            IExtratoBancarioItemService extratoBancarioItemService)
        {
            _extratoBancarioItemService = extratoBancarioItemService;
        }

        // GET /GetExtratoBancarioItens/{extratoId}
        [HttpGet("/GetExtratoBancarioItens/{extratoId:int}")]
        public async Task<ActionResult<IEnumerable<ExtratoBancarioItemDTO>>> GetExtratoItens(int extratoId)
        {
            var userId = UsuarioId;

            var itens = await _extratoBancarioItemService.GetByExtratoAsync(extratoId, userId);
            return Ok(itens);
        }

        // GET /GetExtratoBancarioItemById/{id}
        [HttpGet("/GetExtratoBancarioItemById/{id:int}", Name = "GetExtratoBancarioItemById")]
        public async Task<ActionResult<ExtratoBancarioItemDTO>> GetExtratoItemById(int id)
        {
            var userId = UsuarioId;

            var item = await _extratoBancarioItemService.GetByIdAsync(id, userId);
            if (item == null)
                return NotFound("Item de extrato não encontrado.");

            return Ok(item);
        }

        // POST /CreateExtratoBancarioItem
        [HttpPost("/CreateExtratoBancarioItem")]
        public async Task<ActionResult> CreateExtratoItem([FromBody] ExtratoBancarioItemDTO dto)
        {
            var userId = UsuarioId;

            if (dto == null)
                return BadRequest("Dados inválidos.");

            var created = await _extratoBancarioItemService.AddAsync(dto, userId);
            return CreatedAtRoute("GetExtratoBancarioItemById", new { id = created.Id }, created);
        }

        // PUT /UpdateExtratoBancarioItem
        [HttpPut("/UpdateExtratoBancarioItem")]
        public async Task<ActionResult> UpdateExtratoItem([FromBody] ExtratoBancarioItemDTO dto)
        {
            var userId = UsuarioId;

            if (dto == null || dto.Id <= 0)
                return BadRequest("Dados inválidos.");

            await _extratoBancarioItemService.UpdateAsync(dto, userId);
            
            return Ok(new { message = "Item de extrato atualizado com sucesso." });
        }

        // DELETE /DeleteExtratoBancarioItem/{id}
        [HttpDelete("/DeleteExtratoBancarioItem/{id:int}")]
        public async Task<ActionResult> DeleteExtratoItem(int id)
        {
            var userId = UsuarioId;

            var item = await _extratoBancarioItemService.GetByIdAsync(id, userId);
            if(item == null)
                return NotFound(new {message = "Item de extrato não encontrado."});

            await _extratoBancarioItemService.RemoveAsync(id, userId);
            return Ok(item);
        }

        /// <summary>
        /// Retorna os itens de extrato do usuário por mês (month = yyyy-MM).
        /// </summary>
        // GET /GetExtratoBancarioItensByMonth?month=2025-11
        [HttpGet("/GetExtratoBancarioItensByMonth")]
        public async Task<ActionResult<IEnumerable<ExtratoBancarioItemDTO>>> GetByMonth(
            [FromQuery] string month, [FromQuery] int? bancoId)
        {
            var userId = UsuarioId;

            if (string.IsNullOrWhiteSpace(month))
                return BadRequest("Parâmetro 'month' é obrigatório no formato yyyy-MM.");

            // Tenta montar um dia 01 desse mês
            if (!DateTime.TryParseExact(
                    month + "-01",
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var dt))
            {
                return BadRequest("Formato de mês inválido. Use yyyy-MM (ex.: 2025-11).");
            }
            var itens = await _extratoBancarioItemService.GetByMonthAsync(userId, dt.Year, dt.Month, bancoId ?? null);
            return Ok(itens);
        }

    }
}
