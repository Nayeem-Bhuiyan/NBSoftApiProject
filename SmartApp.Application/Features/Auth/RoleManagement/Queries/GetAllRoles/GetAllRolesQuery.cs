using MediatR;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.RoleManagement.Queries.GetAllRoles;

public sealed record GetAllRolesQuery : IRequest<Response<List<RoleDto>>>;