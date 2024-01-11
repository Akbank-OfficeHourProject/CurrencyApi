using Currency.Api.CQRS;
using Currency.Api.Response;
using Currency.Api.Schema;
using MediatR;
using Newtonsoft.Json;

namespace Currency.Api.Handler;

public class
    CurrencyConversionHandler : IRequestHandler<ConvertCurrencyCommand, ApiResponse<CurrencyConversionResponse>>
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public CurrencyConversionHandler(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<ApiResponse<CurrencyConversionResponse>> Handle(ConvertCurrencyCommand request,
        CancellationToken cancellationToken)
    {
        var apiKey = _configuration["OpenExchangeRates:ApiKey"];
        var baseUrl = _configuration["OpenExchangeRates:BaseUrl"];
        var url = $"{baseUrl}latest.json?app_id={apiKey}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return new ApiResponse<CurrencyConversionResponse>("Error retrieving data from OpenExchangeRates");

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var latestRatesResponse = JsonConvert.DeserializeObject<LatestRatesResponse>(content);

        var fromRateResponse = GetRateForCurrency(request.Model.FromCurrency, latestRatesResponse.Rates);
        if (!fromRateResponse.Success)
            return new ApiResponse<CurrencyConversionResponse>(fromRateResponse.Message);

        var toRateResponse = GetRateForCurrency(request.Model.ToCurrency, latestRatesResponse.Rates);
        if (!toRateResponse.Success)
            return new ApiResponse<CurrencyConversionResponse>(toRateResponse.Message);

        var convertedAmount = (toRateResponse.Response / fromRateResponse.Response) * request.Model.Amount;

        return new ApiResponse<CurrencyConversionResponse>(new CurrencyConversionResponse
            { ConvertedAmount = convertedAmount });
    }

    private ApiResponse<decimal> GetRateForCurrency(string targetCurrency, Dictionary<string, decimal> rates)
    {
        return rates.TryGetValue(targetCurrency, out decimal rate)
            ? new ApiResponse<decimal>(rate)
            : new ApiResponse<decimal>("Currency not found.");
    }
}