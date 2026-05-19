using FluentValidation;

namespace SmartApp.Application.Features.MasterData.Countries.Commands.DeleteCountry;

public sealed class DeleteCountryValidator : AbstractValidator<DeleteCountryCommand>
{
    public DeleteCountryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Valid country ID is required.");
    }
}