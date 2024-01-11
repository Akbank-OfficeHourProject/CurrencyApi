namespace Currency.Api.Schema;

public class CurrencyConversionRequest
{
    public decimal Amount { get; set; } 
    public string FromCurrency { get; set; }
    public string ToCurrency { get; set; }
}

public class CurrencyConversionResponse
{
    public decimal ConvertedAmount { get; set; }
}