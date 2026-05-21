using MediatR;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.UserManagement.Commands.AdminResetPassword;

public sealed record AdminResetPasswordCommand(
    string UserId,
    string NewPassword
) : IRequest<Response<bool>>;