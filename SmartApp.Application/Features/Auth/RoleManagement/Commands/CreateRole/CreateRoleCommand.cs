using MediatR;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.RoleManagement.Commands.CreateRole;

public sealed record CreateRoleCommand(
    string Name,
    string Description
) : IRequest<Response<RoleDto>>;