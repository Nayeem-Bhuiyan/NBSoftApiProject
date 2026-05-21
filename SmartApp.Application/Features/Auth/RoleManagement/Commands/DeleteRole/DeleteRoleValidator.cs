using FluentValidation;

namespace SmartApp.Application.Features.Auth.RoleManagement.Commands.DeleteRole;

public sealed class DeleteRoleValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Role ID is required.");
    }
}