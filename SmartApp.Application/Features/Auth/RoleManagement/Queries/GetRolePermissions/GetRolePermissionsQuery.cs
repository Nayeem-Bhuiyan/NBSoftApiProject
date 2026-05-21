using MediatR;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.RoleManagement.Queries.GetRolePermissions;

public sealed record GetRolePermissionsQuery(string RoleId) : IRequest<Response<List<RolePermissionDto>>>;