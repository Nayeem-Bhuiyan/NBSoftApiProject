using MediatR;
using SmartApp.Application.DTOs.MasterData.Country;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.MasterData.Countries.Queries.GetCountryById;

public sealed record GetCountryByIdQuery(int Id) : IRequest<Response<CountryDto>>;