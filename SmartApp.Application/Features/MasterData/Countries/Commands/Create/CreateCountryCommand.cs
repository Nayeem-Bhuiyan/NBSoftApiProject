using MediatR;
using SmartApp.Application.DTOs.MasterData.Country;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.MasterData.Countries.Commands.CreateCountry;

public sealed record CreateCountryCommand(
    string Name,
    string Code,
    string Continent,
    string Currency,
    string PhoneCode,
    bool IsActive = true
) : IRequest<Response<CountryDto>>;