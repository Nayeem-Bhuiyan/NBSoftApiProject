using MediatR;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.RoleManagement.Commands.BulkAssignPermissions;

public sealed record BulkAssignPermissionsCommand(
    string RoleId,
    string Controller,
    bool IsGranted
) : IRequest<Response<bool>>;