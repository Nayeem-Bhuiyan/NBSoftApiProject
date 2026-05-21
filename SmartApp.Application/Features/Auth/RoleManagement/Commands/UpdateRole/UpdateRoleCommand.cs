using MediatR;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.RoleManagement.Commands.UpdateRole;

public sealed record UpdateRoleCommand(
    string Id,
    string Name,
    string Description
) : IRequest<Response<RoleDto>>;