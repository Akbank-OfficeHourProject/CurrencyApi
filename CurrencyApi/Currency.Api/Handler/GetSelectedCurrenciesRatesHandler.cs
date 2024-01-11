using Currency.Api.CQRS;
using Currency.Api.Response;
using Currency.Api.Schema;
using MediatR;
using Newtonsoft.Json;

namespace Currency.Api.Handler;

public class GetSelectedCurrenciesRatesHandler : IRequestHandler<GetSelectedCurrenciesRatesQuery,
    ApiResponse<SelectedCurrenciesRatesResponse>>
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GetSelectedCurrenciesRatesHandler(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<ApiResponse<SelectedCurrenciesRatesResponse>> Handle(GetSelectedCurrenciesRatesQuery request,
        CancellationToken cancellationToken)
    {
        var apiKey = _configuration["OpenExchangeRates:ApiKey"];
        var baseUrl = _configuration["OpenExchangeRates:BaseUrl"];
        var url = $"{baseUrl}latest.json?app_id={apiKey}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return new ApiResponse<SelectedCurrenciesRatesResponse>("Error retrieving data from OpenExchangeRates");

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var latestRatesResponse = JsonConvert.DeserializeObject<LatestRatesResponse>(content);

        if (!latestRatesResponse.Rates.TryGetValue(request.BaseCurrency, out var baseRateInUsd) || baseRateInUsd == 0)
            return new ApiResponse<SelectedCurrenciesRatesResponse>($"Base currency {request.BaseCurrency} not found or rate is zero.");


        if (baseRateInUsd == 0)
            return new ApiResponse<SelectedCurrenciesRatesResponse>($"Base currency {request.BaseCurrency} not found.");

        var convertedRates = request.TargetCurrencies
            .Where(currency => latestRatesResponse.Rates.ContainsKey(currency))
            .ToDictionary(currency => currency, currency => latestRatesResponse.Rates[currency] / baseRateInUsd);

        return new ApiResponse<SelectedCurrenciesRatesResponse>(new SelectedCurrenciesRatesResponse
            { Rates = convertedRates });
    }
}