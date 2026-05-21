using FluentValidation;

namespace SmartApp.Application.Features.Auth.UserManagement.Commands.AdminResetPassword;

public sealed class AdminResetPasswordValidator : AbstractValidator<AdminResetPasswordCommand>
{
    public AdminResetPasswordValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(4).WithMessage("Password must be at least 4 characters.");
    }
}