using MediatR;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.UserManagement.Commands.RevokeUserRole;

public sealed record RevokeUserRoleCommand(
    string UserId,
    string RoleName
) : IRequest<Response<bool>>;