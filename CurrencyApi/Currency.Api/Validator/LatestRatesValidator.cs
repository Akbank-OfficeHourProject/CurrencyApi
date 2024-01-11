using Currency.Api.Schema;
using FluentValidation;

namespace Currency.Api.Validator;

public class LatestRatesValidator : AbstractValidator<LatestRatesRequest>
{
    public LatestRatesValidator()
    {
        RuleFor(request => request.BaseCurrency)
            .Length(3).When(request => !string.IsNullOrEmpty(request.BaseCurrency))
            .WithMessage("BaseCurrency must be 3 characters")
            .Matches("^[A-Z]+$").When(request => !string.IsNullOrEmpty(request.BaseCurrency))
            .WithMessage("BaseCurrency must be in uppercase");
    }
}