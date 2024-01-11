using Currency.Api.Schema;
using FluentValidation;

namespace Currency.Api.Validator;

public class CurrencyConversionValidator: AbstractValidator<CurrencyConversionRequest>
{
    public CurrencyConversionValidator()
    {
        RuleFor(request => request.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0");

        RuleFor(request => request.FromCurrency)
            .NotEmpty().WithMessage("FromCurrency is required")
            .Length(3).WithMessage("FromCurrency must be 3 characters")
            .Matches("^[A-Z]+$").WithMessage("FromCurrency must be composed of uppercase letters");

        RuleFor(request => request.ToCurrency)
            .NotEmpty().WithMessage("ToCurrency is required")
            .Length(3).WithMessage("ToCurrency must be 3 characters")
            .Matches("^[A-Z]+$").WithMessage("ToCurrency must be composed of uppercase letters");
    }
}