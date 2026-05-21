using FluentValidation;

namespace SmartApp.Application.Features.Auth.RoleManagement.Commands.AssignPermission;

public sealed class AssignPermissionValidator : AbstractValidator<AssignPermissionCommand>
{
    public AssignPermissionValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role ID is required.");

        RuleFor(x => x.PermissionId)
            .GreaterThan(0).WithMessage("Valid permission ID is required.");
    }
}