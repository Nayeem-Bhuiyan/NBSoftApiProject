using MediatR;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.UserManagement.Queries.GetUsersPaged;

public sealed record GetUsersPagedQuery(
    string? SearchTerm,
    string? RoleFilter,
    bool? IsActive,
    int PageIndex,
    int PageSize,
    string? SortBy,
    bool SortDesc
) : IRequest<Response<PagedResult<UserSummaryDto>>>;