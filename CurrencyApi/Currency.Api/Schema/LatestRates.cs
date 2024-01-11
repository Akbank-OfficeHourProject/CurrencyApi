namespace Currency.Api.Schema;

public class LatestRatesRequest
{
    public string? BaseCurrency { get; set; }
}

public class LatestRatesResponse
{
    public string Base { get; set; }
    public Dictionary<string, decimal> Rates { get; set; }
}