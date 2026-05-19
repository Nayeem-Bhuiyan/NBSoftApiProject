using MediatR;
using SmartApp.Application.Interfaces.Repositories;
using SmartApp.Domain.Entities.MasterData;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.MasterData.Countries.Commands.DeleteCountry;

public sealed class DeleteCountryHandler : IRequestHandler<DeleteCountryCommand, Response<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCountryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Response<bool>> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
    {
        var deleteResult = await _unitOfWork.Repository<Country>().DeleteAsync(request.Id, cancellationToken);
        if (!deleteResult.isSuccess)
            return Response<bool>.Failure(deleteResult.message);

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (!saveResult.isSuccess)
            return Response<bool>.Failure(saveResult.message);

        return Response<bool>.SuccessResponse(true, "Country deleted successfully.");
    }
}