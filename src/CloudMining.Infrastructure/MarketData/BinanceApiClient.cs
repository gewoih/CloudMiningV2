using System.Globalization;
using System.Text;
using CloudMining.Application.Utils;
using CloudMining.Domain.Enums;
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

    public async Task<List<PriceData>> GetPriceData(
        CurrencyCode fromCurrency,
        CurrencyCode toCurrency,
        CandlestickTimeFrame timeFrame = CandlestickTimeFrame.Hour,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int limit = 500)
    {
        var symbol = $"{fromCurrency}{toCurrency}";
        var requestUrl = string.Format(_getPriceDataUrl, symbol, timeFrame.GetDescription(), limit);

        if (fromDate.HasValue)
        {
            var unixEpoch = DateTime.UnixEpoch;
            var startTime = fromDate.Value.ToUniversalTime();
            var startTimeMilliseconds = (long)(startTime - unixEpoch).TotalMilliseconds;
            requestUrl += $"&startTime={startTimeMilliseconds}";
        }

        if (toDate.HasValue)
        {
            var unixEpoch = DateTime.UnixEpoch;
            var endTime = toDate.Value.ToUniversalTime();
            var endTimeMilliseconds = (long)(endTime - unixEpoch).TotalMilliseconds;
            requestUrl += $"&endTime={endTimeMilliseconds}";
        }

        var response = await _httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<List<List<object>>>(responseContent);

        var priceDataList = new List<PriceData>();
        foreach (var entry in data)
        {
            var unixTime = Convert.ToInt64(entry[0]);
            var date = DateTimeOffset.FromUnixTimeMilliseconds(unixTime).UtcDateTime;
            var priceString = entry[4].ToString();
            var price = decimal.Parse(priceString!, CultureInfo.InvariantCulture);
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