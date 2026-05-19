using MediatR;
using SmartApp.Application.DTOs.MasterData.Country;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.MasterData.Countries.Commands.UpdateCountry;

public sealed record UpdateCountryCommand(
    int Id,
    string Name,
    string Code,
    string Continent,
    string Currency,
    string PhoneCode,
    bool IsActive
) : IRequest<Response<CountryDto>>;