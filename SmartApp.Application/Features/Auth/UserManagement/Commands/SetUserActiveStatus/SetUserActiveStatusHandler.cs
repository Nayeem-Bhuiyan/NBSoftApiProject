using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartApp.Application.Interfaces.Auth;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.UserManagement.Commands.SetUserActiveStatus;

public sealed class SetUserActiveStatusHandler : IRequestHandler<SetUserActiveStatusCommand, Response<bool>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRefreshTokenService _refreshTokenService;

    public SetUserActiveStatusHandler(
        UserManager<ApplicationUser> userManager,
        IRefreshTokenService refreshTokenService)
    {
        _userManager         = userManager;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Response<bool>> Handle(SetUserActiveStatusCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null)
            return Response<bool>.Failure("User not found.");

        user.isActive = request.IsActive;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return Response<bool>.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));

        // ← revoke all sessions immediately when user is deactivated
        if (!request.IsActive)
            await _refreshTokenService.RevokeAllUserSessionsAsync(request.UserId, ct);

        return Response<bool>.SuccessResponse(true,
            request.IsActive ? "User activated successfully." : "User deactivated successfully.");
    }
}