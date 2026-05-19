using AutoMapper;
using MediatR;
using SmartApp.Application.DTOs.MasterData.Country;
using SmartApp.Application.Interfaces.Repositories;
using SmartApp.Domain.Entities.MasterData;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.MasterData.Countries.Commands.UpdateCountry;

public sealed class UpdateCountryHandler : IRequestHandler<UpdateCountryCommand, Response<CountryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateCountryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper     = mapper;
    }

    public async Task<Response<CountryDto>> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
    {
        var existing = await _unitOfWork.Repository<Country>().GetByIdAsync(request.Id, cancellationToken);
        if (!existing.isSuccess || existing.data is null)
            return Response<CountryDto>.Failure("Country not found.");

        _mapper.Map(request, existing.data);

        var updateResult = await _unitOfWork.Repository<Country>().UpdateAsync(existing.data, cancellationToken);
        if (!updateResult.isSuccess)
            return Response<CountryDto>.Failure(updateResult.message);

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (!saveResult.isSuccess)
            return Response<CountryDto>.Failure(saveResult.message);

        return Response<CountryDto>.SuccessResponse(_mapper.Map<CountryDto>(existing.data), "Country updated successfully.");
    }
}