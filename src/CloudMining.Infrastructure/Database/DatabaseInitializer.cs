using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Domain.Models.Identity;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Interfaces.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Infrastructure.Database;

public sealed class DatabaseInitializer
{
	private readonly UserManager<User> _userManager;
	private readonly RoleManager<Role> _roleManager;
	private readonly ICurrencyService _currencyService;
	private readonly IUserManagementService _userManagementService;
	private readonly CloudMiningContext _context;

	public DatabaseInitializer(UserManager<User> userManager, 
		RoleManager<Role> roleManager,
		ICurrencyService currencyService, 
		IUserManagementService userManagementService, 
		CloudMiningContext context)
	{
		_userManager = userManager;
		_roleManager = roleManager;
		_currencyService = currencyService;
		_userManagementService = userManagementService;
		_context = context;
	}

	public async Task InitializeAsync()
	{
		await CreateEthPayoutsAsync();
		await CreateRolesAsync();
		await CreateUsersAsync();
	}

	public static List<Currency> GetCurrencies()
	{
		return
		[
			new Currency
			{
				Id = new Guid("E23A58C6-9CEF-4C6F-94FC-8577A4B4FB84"), Caption = "Рубль", Code = CurrencyCode.RUB,
				ShortName = "₽", Precision = 2
			},
			new Currency
			{
				Id = new Guid("F1DEBADF-A2C3-4908-A11C-8329DF252FB8"), Caption = "Доллар", Code = CurrencyCode.USD,
				ShortName = "$", Precision = 2
			},
			new Currency
			{
				Id = new Guid("9A3E4016-240E-44E3-A002-0EE2CFF499A3"), Caption = "Bitcoin", Code = CurrencyCode.BTC,
				ShortName = "BTC", Precision = 5
			},
			new Currency
			{
				Id = new Guid("8927702D-AE2E-4A5A-AF19-2E6FA1824648"), Caption = "Etherium", Code = CurrencyCode.ETH,
				ShortName = "ETH", Precision = 4
			},
			new Currency
			{
				Id = new Guid("62C520F6-499A-499E-86C2-B2AF9C8EBA42"), Caption = "Litecoin", Code = CurrencyCode.LTC,
				ShortName = "LTC", Precision = 2
			},
			new Currency
			{
				Id = new Guid("C24B466A-97C2-4D64-BBE7-C583B76A2C3C"), Caption = "Tether", Code = CurrencyCode.USDT,
				ShortName = "USDT", Precision = 2
			},
			new Currency
			{
				Id = new Guid("A5450179-3BFF-4645-9209-04ACC6168C5B"), Caption = "Dogecoin", Code = CurrencyCode.DOGE,
				ShortName = "DOGE", Precision = 0
			}
		];
	}

	private async Task CreateEthPayoutsAsync()
	{
		if (await _context.ShareablePayments.AnyAsync(payment => payment.Currency.Code == CurrencyCode.ETH))
			return;

		var ethCurrencyId = await _currencyService.GetIdAsync(CurrencyCode.ETH);

		var users = await _userManagementService.GetUsersAsync();
		var nikita = users.First(user => user.FirstName == "Никита");
		var maksimGrigoriev = users.First(user => user is { FirstName: "Максим", LastName: "Григорьев" });
		var egor = users.First(user => user.FirstName == "Егор");
		var ivan = users.First(user => user.FirstName == "Иван");
		var maksimRanenko = users.First(user => user is { FirstName: "Максим", LastName: "Раненко" });
		var maksimGlinkin = users.First(user => user is { FirstName: "Максим", LastName: "Глинкин" });
		var filipp = users.First(user => user.FirstName == "Филипп");

		var payouts = new List<ShareablePayment>
		{
			new()
			{
				Id = Guid.NewGuid(),
				Amount = 0.102m,
				CurrencyId = ethCurrencyId,
				Type = PaymentType.Crypto,
				Date = new DateTime(2021, 7, 5, 12, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0143m,
						Share = 9.78m,
						UserId = nikita.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0517m,
						Share = 54.42m,
						UserId = maksimGrigoriev.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0341m,
						Share = 35.89m,
						UserId = egor.Id,
						Status = ShareStatus.Completed,
					}
				}
			},
			new()
			{
				Id = Guid.NewGuid(),
				Amount = 0.1056m,
				CurrencyId = ethCurrencyId,
				Type = PaymentType.Crypto,
				Date = new DateTime(2021, 7, 17, 12, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0143m,
						Share = 9.78m,
						UserId = nikita.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0517m,
						Share = 54.42m,
						UserId = maksimGrigoriev.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0341m,
						Share = 35.89m,
						UserId = egor.Id,
						Status = ShareStatus.Completed,
					}
				}
			},
			new()
			{
				Id = Guid.NewGuid(),
				Amount = 0.1113m,
				CurrencyId = ethCurrencyId,
				Type = PaymentType.Crypto,
				Date = new DateTime(2021, 8, 1, 12, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0155m,
						Share = 9.78m,
						UserId = nikita.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0564m,
						Share = 54.42m,
						UserId = maksimGrigoriev.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.037m,
						Share = 35.89m,
						UserId = egor.Id,
						Status = ShareStatus.Completed,
					}
				}
			},
			new()
			{
				Id = Guid.NewGuid(),
				Amount = 0.111m,
				CurrencyId = ethCurrencyId,
				Type = PaymentType.Crypto,
				Date = new DateTime(2021, 8, 14, 12, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0316m,
						Share = 25.79m,
						UserId = nikita.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0352m,
						Share = 34.78m,
						UserId = maksimGrigoriev.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0223m,
						Share = 22.03m,
						UserId = egor.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0078m,
						Share = 7.7m,
						UserId = ivan.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0128m,
						Share = 9.38m,
						UserId = maksimRanenko.Id,
						Status = ShareStatus.Completed,
					}
				}
			},
			new()
			{
				Id = Guid.NewGuid(),
				Amount = 0.128m,
				CurrencyId = ethCurrencyId,
				Type = PaymentType.Crypto,
				Date = new DateTime(2021, 9, 10, 13, 24, 52, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0391m,
						Share = 27.81m,
						UserId = nikita.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0306m,
						Share = 25.99m,
						UserId = maksimGrigoriev.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0193m,
						Share = 16.43m,
						UserId = egor.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0068m,
						Share = 5.8m,
						UserId = ivan.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.022m,
						Share = 15.5m,
						UserId = maksimRanenko.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0099m,
						Share = 8.47m,
						UserId = maksimGlinkin.Id,
						Status = ShareStatus.Completed,
					}
				}
			},
			new()
			{
				Id = Guid.NewGuid(),
				Amount = 0.1006m,
				CurrencyId = ethCurrencyId,
				Type = PaymentType.Crypto,
				Date = new DateTime(2021, 10, 5, 13, 53, 11, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0307m,
						Share = 27.81m,
						UserId = nikita.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.024m,
						Share = 25.99m,
						UserId = maksimGrigoriev.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0152m,
						Share = 16.43m,
						UserId = egor.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0053m,
						Share = 5.8m,
						UserId = ivan.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0173m,
						Share = 15.5m,
						UserId = maksimRanenko.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0078m,
						Share = 8.47m,
						UserId = maksimGlinkin.Id,
						Status = ShareStatus.Completed,
					}
				}
			},
			new()
			{
				Id = Guid.NewGuid(),
				Amount = 0.1009m,
				CurrencyId = ethCurrencyId,
				Type = PaymentType.Crypto,
				Date = new DateTime(2021, 10, 28, 13, 54, 43, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0308m,
						Share = 27.83m,
						UserId = nikita.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0241m,
						Share = 25.98m,
						UserId = maksimGrigoriev.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0152m,
						Share = 16.43m,
						UserId = egor.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0053m,
						Share = 5.8m,
						UserId = ivan.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0174m,
						Share = 15.49m,
						UserId = maksimRanenko.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0078m,
						Share = 8.47m,
						UserId = maksimGlinkin.Id,
						Status = ShareStatus.Completed,
					}
				}
			},
			new()
			{
				Id = Guid.NewGuid(),
				Amount = 0.1013m,
				CurrencyId = ethCurrencyId,
				Type = PaymentType.Crypto,
				Date = new DateTime(2021, 11, 24, 13, 44, 49, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0291m,
						Share = 25.81m,
						UserId = nikita.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0223m,
						Share = 23.93m,
						UserId = maksimGrigoriev.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0169m,
						Share = 18.17m,
						UserId = egor.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0049m,
						Share = 5.34m,
						UserId = ivan.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0163m,
						Share = 14.27m,
						UserId = maksimRanenko.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0072m,
						Share = 7.8m,
						UserId = maksimGlinkin.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0043m,
						Share = 4.7m,
						UserId = filipp.Id,
						Status = ShareStatus.Completed
					}
				}
			},
			new()
			{
				Id = Guid.NewGuid(),
				Amount = 0.102m,
				CurrencyId = ethCurrencyId,
				Type = PaymentType.Crypto,
				Date = new DateTime(2021, 12, 22, 13, 31, 25, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0292m,
						Share = 25.76m,
						UserId = nikita.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0224m,
						Share = 23.88m,
						UserId = maksimGrigoriev.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.017m,
						Share = 18.13m,
						UserId = egor.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.005m,
						Share = 5.36m,
						UserId = ivan.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0165m,
						Share = 14.34m,
						UserId = maksimRanenko.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0073m,
						Share = 7.84m,
						UserId = maksimGlinkin.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0044m,
						Share = 4.69m,
						UserId = filipp.Id,
						Status = ShareStatus.Completed
					}
				}
			},
			new()
			{
				Id = Guid.NewGuid(),
				Amount = 0.1011m,
				CurrencyId = ethCurrencyId,
				Type = PaymentType.Crypto,
				Date = new DateTime(2022, 1, 19, 13, 57, 21, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0289m,
						Share = 25.72m,
						UserId = nikita.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0223m,
						Share = 24.01m,
						UserId = maksimGrigoriev.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0168m,
						Share = 18.1m,
						UserId = egor.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0049m,
						Share = 5.35m,
						UserId = ivan.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0163m,
						Share = 14.31m,
						UserId = maksimRanenko.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0072m,
						Share = 7.83m,
						UserId = maksimGlinkin.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0043m,
						Share = 4.68m,
						UserId = filipp.Id,
						Status = ShareStatus.Completed
					}
				}
			},
			new()
			{
				Id = Guid.NewGuid(),
				Amount = 0.1012m,
				CurrencyId = ethCurrencyId,
				Type = PaymentType.Crypto,
				Date = new DateTime(2022, 2, 18, 13, 14, 54, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0273m,
						Share = 23.92m,
						UserId = nikita.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0207m,
						Share = 22.29m,
						UserId = maksimGrigoriev.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0156m,
						Share = 16.8m,
						UserId = egor.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0046m,
						Share = 4.97m,
						UserId = ivan.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0184m,
						Share = 16.52m,
						UserId = maksimRanenko.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0067m,
						Share = 7.27m,
						UserId = maksimGlinkin.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0076m,
						Share = 8.23m,
						UserId = filipp.Id,
						Status = ShareStatus.Completed
					}
				}
			},
			new()
			{
				Id = Guid.NewGuid(),
				Amount = 0.1011m,
				CurrencyId = ethCurrencyId,
				Type = PaymentType.Crypto,
				Date = new DateTime(2022, 3, 20, 13, 16, 48, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.027m,
						Share = 23.66m,
						UserId = nikita.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0205m,
						Share = 22.05m,
						UserId = maksimGrigoriev.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0154m,
						Share = 16.63m,
						UserId = egor.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0045m,
						Share = 4.92m,
						UserId = ivan.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0192m,
						Share = 17.42m,
						UserId = maksimRanenko.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0066m,
						Share = 7.19m,
						UserId = maksimGlinkin.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0075m,
						Share = 8.14m,
						UserId = filipp.Id,
						Status = ShareStatus.Completed
					}
				}
			},
			new()
			{
				Id = Guid.NewGuid(),
				Amount = 0.1003m,
				CurrencyId = ethCurrencyId,
				Type = PaymentType.Crypto,
				Date = new DateTime(2022, 4, 20, 13, 05, 53, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0268m,
						Share = 23.66m,
						UserId = nikita.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0203m,
						Share = 22.05m,
						UserId = maksimGrigoriev.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0153m,
						Share = 16.63m,
						UserId = egor.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0045m,
						Share = 4.92m,
						UserId = ivan.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.019m,
						Share = 17.42m,
						UserId = maksimRanenko.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0066m,
						Share = 7.19m,
						UserId = maksimGlinkin.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0075m,
						Share = 8.14m,
						UserId = filipp.Id,
						Status = ShareStatus.Completed
					}
				}
			},
			new()
			{
				Id = Guid.NewGuid(),
				Amount = 0.1009m,
				CurrencyId = ethCurrencyId,
				Type = PaymentType.Crypto,
				Date = new DateTime(2022, 5, 22, 12, 12, 4, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.027m,
						Share = 23.66m,
						UserId = nikita.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0204m,
						Share = 22.05m,
						UserId = maksimGrigoriev.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0154m,
						Share = 16.63m,
						UserId = egor.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0045m,
						Share = 4.92m,
						UserId = ivan.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0191m,
						Share = 17.42m,
						UserId = maksimRanenko.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0066m,
						Share = 7.19m,
						UserId = maksimGlinkin.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0075m,
						Share = 8.14m,
						UserId = filipp.Id,
						Status = ShareStatus.Completed
					}
				}
			},
			new()
			{
				Id = Guid.NewGuid(),
				Amount = 0.1008m,
				CurrencyId = ethCurrencyId,
				Type = PaymentType.Crypto,
				Date = new DateTime(2022, 6, 25, 12, 0, 1, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0269m,
						Share = 23.66m,
						UserId = nikita.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0204m,
						Share = 22.05m,
						UserId = maksimGrigoriev.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0154m,
						Share = 16.63m,
						UserId = egor.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0045m,
						Share = 4.92m,
						UserId = ivan.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0191m,
						Share = 17.42m,
						UserId = maksimRanenko.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0066m,
						Share = 7.19m,
						UserId = maksimGlinkin.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0075m,
						Share = 8.14m,
						UserId = filipp.Id,
						Status = ShareStatus.Completed
					}
				}
			},
			new()
			{
				Id = Guid.NewGuid(),
				Amount = 0.202m,
				CurrencyId = ethCurrencyId,
				Type = PaymentType.Crypto,
				Date = new DateTime(2022, 8, 26, 12, 9, 47, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0541m,
						Share = 23.7m,
						UserId = nikita.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.041m,
						Share = 22.09m,
						UserId = maksimGrigoriev.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0309m,
						Share = 16.65m,
						UserId = egor.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0091m,
						Share = 4.92m,
						UserId = ivan.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0381m,
						Share = 17.29m,
						UserId = maksimRanenko.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0133m,
						Share = 7.2m,
						UserId = maksimGlinkin.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0151m,
						Share = 8.15m,
						UserId = filipp.Id,
						Status = ShareStatus.Completed
					}
				}
			},
			new()
			{
				Id = Guid.NewGuid(),
				Amount = 0.202m,
				CurrencyId = ethCurrencyId,
				Type = PaymentType.Crypto,
				Date = new DateTime(2022, 9, 20, 14, 10, 27, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.01698m,
						Share = 23.67m,
						UserId = nikita.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0129m,
						Share = 22.06m,
						UserId = maksimGrigoriev.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.0097m,
						Share = 16.63m,
						UserId = egor.Id,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(),
						Amount = 0.012m,
						Share = 17.42m,
						UserId = maksimRanenko.Id,
						Status = ShareStatus.Completed,
					},
				}
			},
		};

		await _context.ShareablePayments.AddRangeAsync(payouts);
		await _context.SaveChangesAsync();
	}

	private async Task CreateRolesAsync()
	{
		foreach (var role in Enum.GetValues<UserRole>())
		{
			var roleName = Enum.GetName(role);
			var roleExist = await _roleManager.RoleExistsAsync(roleName);
			if (roleExist)
				continue;

			var roleCommissionPercent = role switch
			{
				UserRole.Admin => 0.05m,
				UserRole.Manager => 0.03m,
				_ => 0m
			};

			await _roleManager.CreateAsync(new Role { Name = roleName, CommissionPercent = roleCommissionPercent });
		}
	}

	private async Task CreateUsersAsync()
	{
		await RegisterUserIfNotExists("Admin",
			"nranenko@bk.ru",
			"Никита",
			"Раненко",
			"Максимович",
			"24042001Nr.",
			new DateTime(2021, 6, 17, 0, 0, 0, DateTimeKind.Utc));

		await RegisterUserIfNotExists("User",
			"grigmaks2014@yandex.ru",
			"Максим",
			"Григорьев",
			"Владимирович",
			"24042001Nr.",
			new DateTime(2021, 6, 15, 0, 0, 0, DateTimeKind.Utc));

		await RegisterUserIfNotExists("User",
			"test4@mail.ru",
			"Егор",
			"Коняев",
			"Сергеевич",
			"24042001Nr.",
			new DateTime(2021, 6, 17, 0, 0, 0, DateTimeKind.Utc));

		await RegisterUserIfNotExists("User",
			"test3@mail.ru",
			"Иван",
			"Кулагин",
			"Владимирович",
			"24042001Nr.",
			new DateTime(2021, 7, 19, 0, 0, 0, DateTimeKind.Utc));

		await RegisterUserIfNotExists("Manager",
			"test2@mail.ru",
			"Максим",
			"Раненко",
			"Владимирович",
			"24042001Nr.",
			new DateTime(2021, 7, 28, 0, 0, 0, DateTimeKind.Utc));

		await RegisterUserIfNotExists("User",
			"test1@mail.ru",
			"Максим",
			"Глинкин",
			"Олегович",
			"24042001Nr.",
			new DateTime(2021, 8, 24, 0, 0, 0, DateTimeKind.Utc));

		await RegisterUserIfNotExists("User",
			"test@mail.ru",
			"Филипп",
			"Евсеев",
			"Филипович",
			"24042001Nr.",
			new DateTime(2021, 11, 11, 0, 0, 0, DateTimeKind.Utc));
	}

	private async Task RegisterUserIfNotExists(string role, string email, string firstName,
		string lastName, string patronymic, string password, DateTime registrationDate)
	{
		var user = new User
		{
			UserName = email,
			Email = email,
			FirstName = firstName,
			LastName = lastName,
			Patronymic = patronymic,
			RegistrationDate = registrationDate
		};

		var userExist = await _userManager.FindByEmailAsync(email);
		if (userExist == null)
		{
			var createUser = await _userManager.CreateAsync(user, password);
			if (createUser.Succeeded)
				await _userManager.AddToRoleAsync(user, role);
		}
	}
}