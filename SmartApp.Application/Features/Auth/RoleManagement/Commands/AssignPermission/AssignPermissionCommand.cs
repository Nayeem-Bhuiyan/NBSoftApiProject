using MediatR;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.RoleManagement.Commands.AssignPermission;

public sealed record AssignPermissionCommand(
    string RoleId,
    int PermissionId,
    bool IsGranted
) : IRequest<Response<bool>>;