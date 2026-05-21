using MediatR;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Application.Interfaces.Repositories;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.UserManagement.Queries.GetUsersPaged;

public sealed class GetUsersPagedHandler : IRequestHandler<GetUsersPagedQuery, Response<PagedResult<UserSummaryDto>>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersPagedHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Response<PagedResult<UserSummaryDto>>> Handle(GetUsersPagedQuery request, CancellationToken ct)
    {
        return await _userRepository.GetPagedAsync(
            request.SearchTerm,
            request.RoleFilter,
            request.IsActive,
            request.PageIndex,
            request.PageSize,
            request.SortBy,
            request.SortDesc,
            ct);
    }
}