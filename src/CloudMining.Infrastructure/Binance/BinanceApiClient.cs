using System.Globalization;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Utils;
using CloudMining.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

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

    public async Task<List<PriceData>> GetMarketDataAsync(
        CurrencyCode fromCurrency,
        CurrencyCode toCurrency,
        CandlestickTimeFrame timeFrame = CandlestickTimeFrame.Hour,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int limit = 500)
    {
        var requestUrl = GetRequestUrl(fromCurrency, toCurrency, timeFrame, fromDate, toDate, limit);
        var response = await _httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<List<List<object>>>(responseContent);

        return GetPriceDataList(data);
    }

    private string GetRequestUrl(CurrencyCode fromCurrency,
        CurrencyCode toCurrency,
        CandlestickTimeFrame timeFrame,
        DateTime? fromDate,
        DateTime? toDate,
        int limit)
    {
        var symbol = $"{fromCurrency}{toCurrency}";
        var requestUrl = string.Format(_getPriceDataUrl, symbol, timeFrame.GetDescription(), limit);

        if (fromDate.HasValue)
        {
            var fromDateUnix = ((DateTimeOffset)fromDate).ToUnixTimeMilliseconds();
            requestUrl += $"&startTime={fromDateUnix}";
        }

        if (toDate.HasValue)
        {
            var toDateUnix = ((DateTimeOffset)toDate).ToUnixTimeMilliseconds();
            requestUrl += $"&endTime={toDateUnix}";
        }

        return requestUrl;
    }

    private static List<PriceData> GetPriceDataList(List<List<object>>? data)
    {
        var priceDataList = new List<PriceData>();
        foreach (var entry in data)
        {
            var unixTime = Convert.ToInt64(entry[0]);
            var date = DateTimeOffset.FromUnixTimeMilliseconds(unixTime).UtcDateTime;
            var price = decimal.Parse(entry[4].ToString(), CultureInfo.InvariantCulture);

            var priceData = new PriceData
            {
                Price = price,
                Date = date
            };
            priceDataList.Add(priceData);
        }

        return priceDataList;
    }
}