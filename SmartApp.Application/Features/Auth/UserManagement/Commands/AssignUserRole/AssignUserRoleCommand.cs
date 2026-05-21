using MediatR;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.UserManagement.Commands.AssignUserRole;

public sealed record AssignUserRoleCommand(
    string UserId,
    string RoleName
) : IRequest<Response<bool>>;