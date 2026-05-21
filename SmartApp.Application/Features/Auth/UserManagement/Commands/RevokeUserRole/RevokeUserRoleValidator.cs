using FluentValidation;

namespace SmartApp.Application.Features.Auth.UserManagement.Commands.RevokeUserRole;

public sealed class RevokeUserRoleValidator : AbstractValidator<RevokeUserRoleCommand>
{
    public RevokeUserRoleValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.RoleName)
            .NotEmpty().WithMessage("Role name is required.");
    }
}