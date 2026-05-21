using MediatR;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Application.Interfaces.Repositories;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.UserManagement.Queries.GetUserById;

public sealed class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, Response<UserDetailDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Response<UserDetailDto>> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await _userRepository.GetDetailByIdAsync(request.UserId, ct);
        if (user is null)
            return Response<UserDetailDto>.Failure("User not found.");

        return Response<UserDetailDto>.SuccessResponse(user, "User retrieved successfully.");
    }
}