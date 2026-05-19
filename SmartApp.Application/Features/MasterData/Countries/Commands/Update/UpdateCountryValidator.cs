using FluentValidation;

namespace SmartApp.Application.Features.MasterData.Countries.Commands.UpdateCountry;

public sealed class UpdateCountryValidator : AbstractValidator<UpdateCountryCommand>
{
    public UpdateCountryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Valid country ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Country name is required.")
            .MaximumLength(100).WithMessage("Country name must not exceed 100 characters.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Country code is required.")
            .MaximumLength(10).WithMessage("Country code must not exceed 10 characters.");

        RuleFor(x => x.Continent)
            .NotEmpty().WithMessage("Continent is required.")
            .MaximumLength(50).WithMessage("Continent must not exceed 50 characters.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .MaximumLength(50).WithMessage("Currency must not exceed 50 characters.");

        RuleFor(x => x.PhoneCode)
            .NotEmpty().WithMessage("Phone code is required.")
            .MaximumLength(10).WithMessage("Phone code must not exceed 10 characters.");
    }
}