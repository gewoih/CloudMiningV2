using System.Globalization;
using System.Text;
using System.Xml;
using CloudMining.Infrastructure.Binance;
using CloudMining.Infrastructure.Settings;
using Microsoft.Extensions.Options;

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

        var responseBytes = await response.Content.ReadAsByteArrayAsync();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var responseXmlContent = Encoding.GetEncoding("windows-1251").GetString(responseBytes);

        var xmlDoc = new XmlDocument
        {
            XmlResolver = null
        };

        xmlDoc.LoadXml(responseXmlContent);

        return GetPriceDataList(xmlDoc);
    }

    public async Task<List<PriceData>> GetDailyMarketDataAsync()
    {
        var dateTime = DateTime.Today;
        var requestUrl = GetRequestUrl(dateTime);
        var response = await _httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();

        var responseXmlContent = await response.Content.ReadAsStringAsync();
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(responseXmlContent);

        return GetDailyPriceDataList(xmlDoc);
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

    private static List<PriceData> GetPriceDataList(XmlNode data)
    {
        var priceDataList = new List<PriceData>();

        var recordNodes = data.SelectNodes("//Record");
        if (recordNodes == null) return priceDataList;

        foreach (var recordNode in recordNodes)
        {
            var xmlDate = ((XmlNode)recordNode).Attributes?["Date"]?.Value;
            var xmlPrice = ((XmlNode)recordNode).SelectSingleNode("Value")?.InnerText;

            if (string.IsNullOrEmpty(xmlDate) || string.IsNullOrEmpty(xmlPrice)) break;
            if (!DateTime.TryParseExact(xmlDate, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out var date)) break;
            if (!decimal.TryParse(xmlPrice.Replace(",", "."), CultureInfo.InvariantCulture, out var price)) break;

            var priceData = new PriceData
            {
                Date = DateTime.SpecifyKind(date, DateTimeKind.Utc),
                Price = price
            };

            priceDataList.Add(priceData);
        }

        return priceDataList;
    }

    private List<PriceData> GetDailyPriceDataList(XmlNode data)
    {
        var priceDataList = new List<PriceData>();

        var xmlDate = data.Attributes?["Date"]?.Value;
        DateTime.TryParseExact(xmlDate, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
            out var date);

        var currencyNode = data.SelectSingleNode($"//Valute[@ID='{_usdCode}']");

        if (currencyNode == null) return priceDataList;
        var xmlPrice = currencyNode.SelectSingleNode("Value")?.InnerText;

        if (string.IsNullOrEmpty(xmlPrice) || !decimal.TryParse(xmlPrice.Replace(",", "."), out var price))
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