using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartApp.Application.Interfaces.Auth;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.UserManagement.Commands.AdminResetPassword;

public sealed class AdminResetPasswordHandler : IRequestHandler<AdminResetPasswordCommand, Response<bool>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRefreshTokenService _refreshTokenService;

    public AdminResetPasswordHandler(
        UserManager<ApplicationUser> userManager,
        IRefreshTokenService refreshTokenService)
    {
        _userManager         = userManager;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Response<bool>> Handle(AdminResetPasswordCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null)
            return Response<bool>.Failure("User not found.");

        // ← remove current password and set new one atomically
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

        if (!result.Succeeded)
            return Response<bool>.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));

        // ← force re-login on all devices after password reset
        await _refreshTokenService.RevokeAllUserSessionsAsync(request.UserId, ct);

        return Response<bool>.SuccessResponse(true, "Password reset successfully. All sessions have been revoked.");
    }
}