using AutoMapper;
using MediatR;
using SmartApp.Application.DTOs.MasterData.Country;
using SmartApp.Application.Interfaces.Repositories;
using SmartApp.Domain.Entities.MasterData;
using SmartApp.Shared.Common;
using System.Linq.Expressions;

namespace SmartApp.Application.Features.MasterData.Countries.Queries.GetCountriesPaged;

public sealed class GetCountriesPagedHandler : IRequestHandler<GetCountriesPagedQuery, Response<PagedResult<CountryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCountriesPagedHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper     = mapper;
    }

    public async Task<Response<PagedResult<CountryDto>>> Handle(GetCountriesPagedQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Country, bool>>? predicate = null;

        if (!string.IsNullOrWhiteSpace(request.Filter))
            predicate = c => c.Name.Contains(request.Filter) || c.Code.Contains(request.Filter);

        var result = await _unitOfWork.Repository<Country>()
            .GetPagedAsync(predicate, request.PageIndex, request.PageSize, request.SortBy, request.SortDesc, cancellationToken);

        if (!result.isSuccess || result.data is null)
            return Response<PagedResult<CountryDto>>.Failure(result.message);

        var dtoItems = _mapper.Map<List<CountryDto>>(result.data.items);

        var pagedDto = new PagedResult<CountryDto>
        {
            items      = dtoItems,
            totalCount = result.data.totalCount,
            pageIndex  = result.data.pageIndex,
            pageSize   = result.data.pageSize
        };

        return Response<PagedResult<CountryDto>>.SuccessResponse(pagedDto, "Data loaded successfully.");
    }
}