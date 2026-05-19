using AutoMapper;
using MediatR;
using SmartApp.Application.DTOs.MasterData.Country;
using SmartApp.Application.Interfaces.Repositories;
using SmartApp.Domain.Entities.MasterData;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.MasterData.Countries.Commands.CreateCountry;

public sealed class CreateCountryHandler : IRequestHandler<CreateCountryCommand, Response<CountryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCountryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper     = mapper;
    }

    public async Task<Response<CountryDto>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Country>(request);

        var addResult = await _unitOfWork.Repository<Country>().AddAsync(entity, cancellationToken);
        if (!addResult.isSuccess)
            return Response<CountryDto>.Failure(addResult.message);

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (!saveResult.isSuccess)
            return Response<CountryDto>.Failure(saveResult.message);

        return Response<CountryDto>.SuccessResponse(_mapper.Map<CountryDto>(entity), "Country created successfully.");
    }
}