using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartApp.Application.Features.MasterData.Countries.Commands.CreateCountry;
using SmartApp.Application.Features.MasterData.Countries.Commands.DeleteCountry;
using SmartApp.Application.Features.MasterData.Countries.Commands.UpdateCountry;
using SmartApp.Application.Features.MasterData.Countries.Queries.GetCountriesPaged;
using SmartApp.Application.Features.MasterData.Countries.Queries.GetCountryById;
using SmartApp.WebApi.RateLimit;

namespace SmartApp.WebApi.Controllers.MasterData;

[RateLimitPolicy("General")]        // ← controller-level default
[ApiController]
[Route("api/[controller]")]
public class CountryController : ControllerBase
{
    private readonly ISender _sender;

    public CountryController(ISender sender)
    {
        _sender = sender;
    }

    [DisableRateLimiting]           // ← override: public lookup, no limit
    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] string filter,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetCountriesPagedQuery(filter, pageIndex, pageSize, sortBy, sortDesc), ct);

        return result.isSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetCountryByIdQuery(id), ct);
        return result.isSuccess ? Ok(result) : NotFound(result);
    }

    [Authorize(Policy = "DynamicPermission")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCountryCommand command, CancellationToken ct = default)
    {
        var result = await _sender.Send(command, ct);
        return result.isSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize(Policy = "DynamicPermission")]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCountryCommand command, CancellationToken ct = default)
    {
        var result = await _sender.Send(command, ct);
        return result.isSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize(Policy = "DynamicPermission")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var result = await _sender.Send(new DeleteCountryCommand(id), ct);
        return result.isSuccess ? Ok(result) : BadRequest(result);
    }
}