using System.Text;
using CloudMining.Domain.Enums;
using CloudMining.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace CloudMining.Infrastructure.Binance;

public sealed class BinanceApiClient
{
    private readonly string _getPriceDataUrl;
    private readonly HttpClient _httpClient;

    public BinanceApiClient(HttpClient httpClient, IOptions<BinanceSettings> settings)
    {
        _httpClient = httpClient;
        
        var baseUrl = settings.Value.BaseUrl;
        _getPriceDataUrl = baseUrl + settings.Value.Endpoints.GetPriceDataUrl;
    }

    public async Task<List<PriceData>> GetPriceData(
        int limit = default,
        long startTime = default,
        long endTime = default, 
        CurrencyCode from = CurrencyCode.BTC, 
        CurrencyCode to = CurrencyCode.RUB,
        CandlestickIntervals candlesInterval = CandlestickIntervals._1h)
    {
        var parameters = new Dictionary<string, string>();

        if (limit != default)
            parameters.Add("limit", limit.ToString());

        if (startTime != default)
            parameters.Add("startTime", startTime.ToString());

        if (endTime != default)
            parameters.Add("endTime", endTime.ToString());
        
        var symbol = $"{from}{to}";
        var interval = candlesInterval.ToString().Substring(1);

        parameters.Add("symbol", symbol);
        parameters.Add("interval", interval);
        
        var queryBuilder = new StringBuilder(_getPriceDataUrl);

        if (parameters.Any())
        {
            queryBuilder.Append("?");
            queryBuilder.Append(string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}")));
        }

        var priceData = await _httpClient.GetAsync(queryBuilder.ToString());
    }
    
}