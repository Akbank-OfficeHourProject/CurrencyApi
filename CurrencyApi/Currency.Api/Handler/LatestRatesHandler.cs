using Currency.Api.CQRS;
using Currency.Api.Response;
using Currency.Api.Schema;
using MediatR;
using Newtonsoft.Json;

namespace Currency.Api.Handler;

public class LatestRatesHandler : IRequestHandler<GetLatestRatesQuery, ApiResponse<LatestRatesResponse>>
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public LatestRatesHandler(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<ApiResponse<LatestRatesResponse>> Handle(GetLatestRatesQuery request,
        CancellationToken cancellationToken)
    {
        var apiKey = _configuration["OpenExchangeRates:ApiKey"];
        var baseUrl = _configuration["OpenExchangeRates:BaseUrl"];
        var url = $"{baseUrl}latest.json?app_id={apiKey}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return new ApiResponse<LatestRatesResponse>("Error retrieving data from OpenExchangeRates");

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var latestRatesResponse = JsonConvert.DeserializeObject<LatestRatesResponse>(content);

        if (latestRatesResponse == null)
            return new ApiResponse<LatestRatesResponse>("Failed to parse the response from OpenExchangeRates");

        var baseCurrency = string.IsNullOrEmpty(request.Request.BaseCurrency) ? "USD" : request.Request.BaseCurrency;

        if (!latestRatesResponse.Rates.TryGetValue(baseCurrency, out decimal value))
            return new ApiResponse<LatestRatesResponse>(
                $"Base currency {baseCurrency} rate not found in OpenExchangeRates");

        var convertedRates = latestRatesResponse.Rates.ToDictionary(
            rate => rate.Key,
            rate => rate.Key == baseCurrency ? 1 : rate.Value / value
        );
        
        return new ApiResponse<LatestRatesResponse>(
            new LatestRatesResponse
            {
                Base = baseCurrency,
                Rates = convertedRates
            }
        );
    }
}