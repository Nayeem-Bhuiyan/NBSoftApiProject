using FluentValidation;

namespace SmartApp.Application.Features.Auth.UserManagement.Commands.SetUserActiveStatus;

public sealed class SetUserActiveStatusValidator : AbstractValidator<SetUserActiveStatusCommand>
{
    public SetUserActiveStatusValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}