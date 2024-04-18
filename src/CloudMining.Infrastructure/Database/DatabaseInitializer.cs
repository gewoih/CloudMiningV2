using CloudMining.Domain.Enums;
using CloudMining.Domain.Models;

namespace CloudMining.Infrastructure.Database
{
	public static class DatabaseInitializer
	{
		public static List<Currency> GetCurrencies()
		{
			return
			[
				new() { Id = Guid.NewGuid(), Caption = "Рубль", Code = CurrencyCode.RUB, Precision = 2 },
				new() { Id = Guid.NewGuid(), Caption = "Доллар", Code = CurrencyCode.USD, Precision = 2 },
				new() { Id = Guid.NewGuid(), Caption = "Bitcoin", Code = CurrencyCode.BTC, Precision = 4 },
				new() { Id = Guid.NewGuid(), Caption = "Etherium", Code = CurrencyCode.ETH, Precision = 4 },
				new() { Id = Guid.NewGuid(), Caption = "Litecoin", Code = CurrencyCode.LTC, Precision = 2 },
				new() { Id = Guid.NewGuid(), Caption = "Dogecoin", Code = CurrencyCode.DOGE, Precision = 0 }
			];
		}
	}
}
