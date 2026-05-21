using MediatR;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.UserManagement.Commands.SetUserActiveStatus;

public sealed record SetUserActiveStatusCommand(
    string UserId,
    bool IsActive
) : IRequest<Response<bool>>;