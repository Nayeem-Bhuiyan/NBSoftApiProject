using Microsoft.AspNetCore.Mvc;
using SmartApp.Application.DTOs.MasterData.Country;
using SmartApp.Application.Interfaces.MasterData;
using SmartApp.Domain.Entities.MasterData;
using SmartApp.Shared.Common;

namespace SmartApp.API.Controllers.MasterData
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] string filter, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _countryService.GetPagedAsync(filter, pageIndex, pageSize);
            if (!result.isSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCountryDto createDto)
        {
            var result = await _countryService.CreateAsync(createDto);
            if (!result.isSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCountryDto updateDto)
        {
            var result = await _countryService.UpdateAsync(updateDto);
            if (!result.isSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _countryService.DeleteAsync(id);
            if (!result.isSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _countryService.GetByIdAsync(id);
            if (!result.isSuccess)
                return NotFound(result);

            return Ok(result);
        }

      
    }
}

