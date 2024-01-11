using Currency.Api.Response;
using Currency.Api.Schema;
using MediatR;

namespace Currency.Api.CQRS;

public record ConvertCurrencyCommand(CurrencyConversionRequest Model)
    : IRequest<ApiResponse<CurrencyConversionResponse>>;

public record GetLatestRatesQuery(LatestRatesRequest Request) : IRequest<ApiResponse<LatestRatesResponse>>;

public record GetSelectedCurrenciesRatesQuery(string BaseCurrency, List<string> TargetCurrencies) : IRequest<ApiResponse<SelectedCurrenciesRatesResponse>>;