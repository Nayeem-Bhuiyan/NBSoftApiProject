using MediatR;
using SmartApp.Application.DTOs.MasterData.Country;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.MasterData.Countries.Queries.GetCountriesPaged;

public sealed record GetCountriesPagedQuery(
    string Filter,
    int PageIndex,
    int PageSize,
    string SortBy,
    bool SortDesc
) : IRequest<Response<PagedResult<CountryDto>>>;