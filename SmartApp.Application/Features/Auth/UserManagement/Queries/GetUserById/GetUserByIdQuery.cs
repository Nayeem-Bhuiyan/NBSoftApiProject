using MediatR;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.UserManagement.Queries.GetUserById;

public sealed record GetUserByIdQuery(string UserId) : IRequest<Response<UserDetailDto>>;