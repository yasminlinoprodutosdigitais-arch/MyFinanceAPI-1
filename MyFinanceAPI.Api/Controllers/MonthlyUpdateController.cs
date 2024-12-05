using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonthlyUpdateController : ControllerBase
    {
        private readonly IMonthlyUpdateService _monthlyUpdateService;

        public MonthlyUpdateController(IMonthlyUpdateService monthlyUpdateService)
        {
            _monthlyUpdateService = monthlyUpdateService;
        }

        [HttpGet("/GetMonthlyUpdate")]
        public async Task<ActionResult<IEnumerable<MonthlyUpdate>>> GetMonthlyUpdate()
        {
            var update = await _monthlyUpdateService.GetMonthlyUpdate();
            if(update is null)
                return NotFound("");
            else
                return Ok(update);
                
        }

        [HttpGet("/GetMonthlyByIdCategory/{id}")]
        public async Task<ActionResult<IEnumerable<MonthlyUpdate>>> GetMonthlyUpdateByCategory(int idCategory)
        {
            if(idCategory == 0 || idCategory < 0)
                return BadRequest("");

            var update = await _monthlyUpdateService.GetMonthlyUpdateByCategory(idCategory);

            if(update is null)
                return NotFound();
            else
                return Ok(update);
        }

        [HttpGet("/GetMonthlyById/{id}", Name="GetUpdate")]
        public async Task<ActionResult<MonthlyUpdate>> GetMonthlyUpdateById(int id)
        {
            if(id == 0 || id < 0)
                return BadRequest();
            var update = await _monthlyUpdateService.GetMonthlyUpdateById(id);

            if(update is null)
                return NotFound();
            else
                return Ok(update);
        }

        [HttpGet("/GetByDate/{date}")]
        public async Task<ActionResult<IEnumerable<MonthlyUpdate>>> GetMonthlyUpdateByDate(DateTime date)
        {
            var update = await _monthlyUpdateService.GetMonthlyUpdateByDate(date);
            if(update is null)
                return NotFound();
            else
                return Ok(update);
        }

        [HttpPost("/CreateMonthlyUpdate")]
        public async Task<ActionResult<MonthlyUpdateDTO>> CreateMonthlyUpdate([FromBody] MonthlyUpdateDTO monthlyUpdateDTO)
        {
            await _monthlyUpdateService.Add(monthlyUpdateDTO);
            return new CreatedAtRouteResult("Getupdate", new {id = monthlyUpdateDTO.Id}, monthlyUpdateDTO);
        }

        [HttpPut("/PutMonthlyUpdate")]
        public async Task<ActionResult<MonthlyUpdateDTO>> UpdateMonthlyUpdate([FromBody] MonthlyUpdateDTO monthlyUpdateDTO)
        {
            await _monthlyUpdateService.Update(monthlyUpdateDTO);
            return monthlyUpdateDTO;
        }

        [HttpDelete("/DeleteMonthlyUpdate/{id}")]
        public async Task<ActionResult<MonthlyUpdateDTO>> DeleteAccountHistory(int id)
        {
            var monthlyUpdate = await _monthlyUpdateService.GetMonthlyUpdateById(id);
            await _monthlyUpdateService.Delete(id);
            return monthlyUpdate;
        }
    }
}
