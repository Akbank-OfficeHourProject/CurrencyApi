using Currency.Api.CQRS;
using Currency.Api.Response;
using Currency.Api.Schema;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Currency.Api.Controller;

[Route("api/[controller]")]
[ApiController]
public class CurrencyController : ControllerBase
{
    private readonly IMediator _mediator;

    public CurrencyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("Convert")]
    public async Task<ApiResponse<CurrencyConversionResponse>> ConvertCurrency(
        [FromBody] CurrencyConversionRequest request)
    {
        var operation = new ConvertCurrencyCommand(request);
        var response = await _mediator.Send(operation);

        return response;
    }
    
    [HttpGet("Rates")]
    public async Task<ApiResponse<LatestRatesResponse>> GetLatestRates([FromQuery] string? baseCurrency)
    {
        var request = new LatestRatesRequest { BaseCurrency = baseCurrency };
        var operation = new GetLatestRatesQuery(request);
        var response = await _mediator.Send(operation);
        return response;
    }
    
    [HttpGet("SelectedCurrencyRates")]
    public async Task<ApiResponse<SelectedCurrenciesRatesResponse>> GetSelectedCurrenciesRates([FromQuery] string baseCurrency, [FromQuery] List<string> targetCurrencies)
    {
        var operation = new GetSelectedCurrenciesRatesQuery(baseCurrency, targetCurrencies);
        var response = await _mediator.Send(operation);
        return response;
    }
}