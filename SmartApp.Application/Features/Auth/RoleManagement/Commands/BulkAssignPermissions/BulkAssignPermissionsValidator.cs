using FluentValidation;

namespace SmartApp.Application.Features.Auth.RoleManagement.Commands.BulkAssignPermissions;

public sealed class BulkAssignPermissionsValidator : AbstractValidator<BulkAssignPermissionsCommand>
{
    public BulkAssignPermissionsValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role ID is required.");

        RuleFor(x => x.Controller)
            .NotEmpty().WithMessage("Controller name is required.");
    }
}