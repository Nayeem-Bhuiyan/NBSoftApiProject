using SmartApp.Application.DTOs.Auth;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<Response<PagedResult<UserSummaryDto>>> GetPagedAsync(
        string? searchTerm,
        string? roleFilter,
        bool? isActive,
        int pageIndex,
        int pageSize,
        string? sortBy,
        bool sortDesc,
        CancellationToken ct = default);

    Task<UserDetailDto?> GetDetailByIdAsync(string userId, CancellationToken ct = default);
}