using MediatR;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.RoleManagement.Queries.GetRoleById;

public sealed record GetRoleByIdQuery(string Id) : IRequest<Response<RoleDto>>;