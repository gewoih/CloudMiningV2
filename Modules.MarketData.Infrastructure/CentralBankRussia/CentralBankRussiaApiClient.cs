using System.Globalization;
using CloudMining.Common.Utils;
using Microsoft.Extensions.Options;
using Modules.MarketData.Infrastructure.Binance;
using Modules.MarketData.Infrastructure.Settings;
using Newtonsoft.Json.Linq;

namespace Modules.MarketData.Infrastructure.CentralBankRussia;

public sealed class CentralBankRussiaApiClient
{
	private const string UsdCode = "R01235";

	private readonly string _getHistoricalPriceDataUrl;
	private readonly string _getDailyPriceDataUrl;
	private readonly HttpClient _httpClient;

	public CentralBankRussiaApiClient(HttpClient httpClient, IOptions<CentralBankRussiaSettings> settings)
	{
		var baseUrl = settings.Value.BaseUrl;
		_getHistoricalPriceDataUrl = baseUrl + settings.Value.Endpoints.GetHistoricalPriceDataUrl;
		_getDailyPriceDataUrl = baseUrl + settings.Value.Endpoints.GetDailyPriceDataUrl;
		_httpClient = httpClient;
	}

	public async Task<List<PriceData>> GetMarketDataAsync(DateTime? fromDate = null, DateTime? toDate = null)
	{
		var requestUrl = GetRequestUrl(fromDate, toDate);
		var response = await _httpClient.GetAsync(requestUrl);
		response.EnsureSuccessStatusCode();

		var responseXmlContent = await response.Content.ReadAsStringAsync();

		var jsonData = XmlUtils.ToJson(responseXmlContent);

		return GetPriceDataList(jsonData);
	}

	private string GetRequestUrl(DateTime? fromDate = null, DateTime? toDate = null)
	{
		string requestUrl;

		if (fromDate.HasValue && toDate.HasValue)
		{
			var formattedFromDate = fromDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
			var formattedToDate = toDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
			requestUrl = string.Format(_getHistoricalPriceDataUrl, formattedFromDate, formattedToDate, UsdCode);
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

		if (!decimal.TryParse(jsonPrice.Replace(",", "."), CultureInfo.InvariantCulture, out var price))
			return;

		var priceData = new PriceData
		{
			Date = DateTime.SpecifyKind(date, DateTimeKind.Utc),
			Price = price
		};

		priceDataList.Add(priceData);
	}
}