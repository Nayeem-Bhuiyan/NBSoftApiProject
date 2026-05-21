using MediatR;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.RoleManagement.Commands.DeleteRole;

public sealed record DeleteRoleCommand(string Id) : IRequest<Response<bool>>;