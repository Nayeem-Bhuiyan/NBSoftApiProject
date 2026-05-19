using AutoMapper;
using MediatR;
using SmartApp.Application.DTOs.MasterData.Country;
using SmartApp.Application.Interfaces.Repositories;
using SmartApp.Domain.Entities.MasterData;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.MasterData.Countries.Queries.GetCountryById;

public sealed class GetCountryByIdHandler : IRequestHandler<GetCountryByIdQuery, Response<CountryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCountryByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper     = mapper;
    }

    public async Task<Response<CountryDto>> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.Repository<Country>().GetByIdAsync(request.Id, cancellationToken);

        if (!result.isSuccess || result.data is null)
            return Response<CountryDto>.Failure("Country not found.");

        return Response<CountryDto>.SuccessResponse(_mapper.Map<CountryDto>(result.data), "Country found.");
    }
}