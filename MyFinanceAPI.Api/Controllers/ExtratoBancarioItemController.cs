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
    [ApiController]
    public class ExtratoBancarioItemController : ControllerBase
    {
        private readonly IExtratoBancarioItemService _extratoBancarioItemService;
        private readonly IUserContextService _userContextService;

        public ExtratoBancarioItemController(
            IExtratoBancarioItemService extratoBancarioItemService,
            IUserContextService userContextService)
        {
            _extratoBancarioItemService = extratoBancarioItemService;
            _userContextService = userContextService;
        }

        // GET /GetExtratoBancarioItens/{extratoId}
        [HttpGet("/GetExtratoBancarioItens/{extratoId:int}")]
        public async Task<ActionResult<IEnumerable<ExtratoBancarioItemDTO>>> GetExtratoItens(int extratoId)
        {
            var userId = _userContextService.GetUserIdFromClaims();
            if (userId == 0)
                return Unauthorized("Usuário não autorizado!");

            var itens = await _extratoBancarioItemService.GetByExtratoAsync(extratoId, userId);
            return Ok(itens);
        }

        // GET /GetExtratoBancarioItemById/{id}
        [HttpGet("/GetExtratoBancarioItemById/{id:int}", Name = "GetExtratoBancarioItemById")]
        public async Task<ActionResult<ExtratoBancarioItemDTO>> GetExtratoItemById(int id)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                var item = await _extratoBancarioItemService.GetByIdAsync(id, userId);
                if (item == null)
                    return NotFound("Item de extrato não encontrado.");

                return Ok(item);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        // POST /CreateExtratoBancarioItem
        [HttpPost("/CreateExtratoBancarioItem")]
        public async Task<ActionResult> CreateExtratoItem([FromBody] ExtratoBancarioItemDTO dto)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                if (dto == null)
                    return BadRequest("Dados inválidos.");

                var created = await _extratoBancarioItemService.AddAsync(dto, userId);
                return CreatedAtRoute("GetExtratoBancarioItemById", new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar item de extrato.", error = ex.Message });
            }
        }

        // PUT /UpdateExtratoBancarioItem
        [HttpPut("/UpdateExtratoBancarioItem")]
        public async Task<ActionResult> UpdateExtratoItem([FromBody] ExtratoBancarioItemDTO dto)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                if (dto == null || dto.Id <= 0)
                    return BadRequest("Dados inválidos.");

                await _extratoBancarioItemService.UpdateAsync(dto, userId);
                return Ok("Item de extrato atualizado com sucesso.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atualizar item de extrato.", error = ex.Message });
            }
        }

        // DELETE /DeleteExtratoBancarioItem/{id}
        [HttpDelete("/DeleteExtratoBancarioItem/{id:int}")]
        public async Task<ActionResult> DeleteExtratoItem(int id)
        {
            try
            {
                var userId = _userContextService.GetUserIdFromClaims();
                if (userId == 0)
                    return Unauthorized("Usuário não autorizado!");

                await _extratoBancarioItemService.RemoveAsync(id, userId);
                return Ok("Item de extrato removido com sucesso.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao remover item de extrato.", error = ex.Message });
            }
        }

        /// <summary>
        /// Retorna os itens de extrato do usuário por mês (month = yyyy-MM).
        /// </summary>
        // GET /GetExtratoBancarioItensByMonth?month=2025-11
        [HttpGet("/GetExtratoBancarioItensByMonth")]
        public async Task<ActionResult<IEnumerable<ExtratoBancarioItemDTO>>> GetByMonth(
            [FromQuery] string month)
        {
            var userId = _userContextService.GetUserIdFromClaims();
            if (userId == 0)
                return Unauthorized("Usuário não autorizado!");

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
            var itens = await _extratoBancarioItemService.GetByMonthAsync(userId, dt.Year, dt.Month);
            return Ok(itens);
        }

    }
}
