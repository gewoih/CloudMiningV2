using System.Globalization;
using System.Text;
using System.Xml;
using CloudMining.Application.Services;
using CloudMining.Infrastructure.Binance;
using CloudMining.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CloudMining.Infrastructure.CentralBankRussia;

public sealed class CentralBankRussiaApiClient
{
    private readonly string _getHistoricalPriceDataUrl;
    private readonly string _getDailyPriceDataUrl;
    private readonly string _usdCode;
    private readonly HttpClient _httpClient;

    public CentralBankRussiaApiClient(HttpClient httpClient, IOptions<CentralBankRussiaSettings> settings)
    {
        _httpClient = httpClient;

        var baseUrl = settings.Value.BaseUrl;
        _getHistoricalPriceDataUrl = baseUrl + settings.Value.Endpoints.GetHistoricalPriceDataUrl;
        _getDailyPriceDataUrl = baseUrl + settings.Value.Endpoints.GetDailyPriceDataUrl;
        _usdCode = settings.Value.UsdCode;
    }

    public async Task<List<PriceData>> GetHistoricalMarketDataAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var requestUrl = GetRequestUrl(fromDate, toDate);
        var response = await _httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();
        
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var responseXmlContent = await response.Content.ReadAsStringAsync();

        var jsonData = CastService.CastXmlToJObject(responseXmlContent);

        return GetPriceDataList(jsonData);
    }

    public async Task<List<PriceData>> GetDailyMarketDataAsync()
    {
        var dateTime = DateTime.Today;
        var requestUrl = GetRequestUrl(dateTime);
        var response = await _httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();

        var responseXmlContent = await response.Content.ReadAsStringAsync();

        var jsonData = CastService.CastXmlToJObject(responseXmlContent);

        return GetDailyPriceDataList(jsonData);
    }

    private string GetRequestUrl(
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        string requestUrl;

        if (fromDate.HasValue && toDate.HasValue)
        {
            var formattedFromDate = fromDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            var formattedToDate = toDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            requestUrl = string.Format(_getHistoricalPriceDataUrl, formattedFromDate, formattedToDate, _usdCode);
        }
        else
        {
            var formattedFromDate = fromDate?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            requestUrl = string.Format(_getDailyPriceDataUrl, formattedFromDate);
        }

        return requestUrl;
    }

    private static List<PriceData> GetPriceDataList(JToken data)
    {
        var priceDataList = new List<PriceData>();

        var valCurs = data["ValCurs"];
        if (valCurs == null)
            return priceDataList;

        var records = valCurs["Record"];
        if (records == null)
            return priceDataList;

        if (records.Type == JTokenType.Array)
        {
            foreach (var record in records)
            {
                AddPriceData(record, priceDataList);
            }
        }
        else
            AddPriceData(records, priceDataList);


        return priceDataList;
    }

    private static void AddPriceData(JToken record, List<PriceData> priceDataList)
    {
        var jsonDate = (string?)record["@Date"];
        var jsonPrice = (string?)record["Value"];

        if (string.IsNullOrEmpty(jsonDate) || string.IsNullOrEmpty(jsonPrice)) return;
        if (!DateTime.TryParseExact(jsonDate, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var date)) return;
        if (!decimal.TryParse(jsonPrice.Replace(",", "."), CultureInfo.InvariantCulture, out var price)) return;

        var priceData = new PriceData
        {
            Date = DateTime.SpecifyKind(date, DateTimeKind.Utc),
            Price = price
        };

        priceDataList.Add(priceData);
    }

    private List<PriceData> GetDailyPriceDataList(JToken data)
    {
        var priceDataList = new List<PriceData>();

        var jsonDate = data["ValCurs"]?["@Date"]?.ToString();
        DateTime.TryParseExact(jsonDate, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
            out var date);

        var currencyToken = data["ValCurs"]?["Valute"].FirstOrDefault(v => (string)v["@ID"] == _usdCode);

        if (currencyToken == null) return priceDataList;
        var jsonPrice = currencyToken["Value"]?.ToString();

        if (string.IsNullOrEmpty(jsonPrice) ||
            !decimal.TryParse(jsonPrice.Replace(",", "."), CultureInfo.InvariantCulture, out var price))
            return priceDataList;


        var priceData = new PriceData
        {
            Date = DateTime.SpecifyKind(date, DateTimeKind.Utc),
            Price = price
        };

        priceDataList.Add(priceData);

        return priceDataList;
    }
    
}