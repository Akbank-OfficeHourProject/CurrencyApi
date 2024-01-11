namespace Currency.Api.Schema;

public class SelectedCurrenciesRatesResponse
{
    public Dictionary<string, decimal> Rates { get; set; }
}