using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Domain.Models.Identity;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Interfaces.DTO.Payments.Deposits;
using CloudMining.Interfaces.DTO.Purchases;
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
	private readonly IDepositService _depositService;
	private readonly CloudMiningContext _context;
	private readonly IPurchaseService _purchaseService;

	public DatabaseInitializer(UserManager<User> userManager,
		RoleManager<Role> roleManager,
		ICurrencyService currencyService,
		IUserManagementService userManagementService,
		CloudMiningContext context,
		IDepositService depositService,
		IPurchaseService purchaseService)
	{
		_userManager = userManager;
		_roleManager = roleManager;
		_currencyService = currencyService;
		_userManagementService = userManagementService;
		_context = context;
		_depositService = depositService;
		_purchaseService = purchaseService;
	}

	public async Task InitializeAsync()
	{
		await InitializeEthPayoutsAsync();
		await InitializeRolesAsync();
		await InitializeUsersAsync();
		await InitializeDepositsAsync();
		await InitializePurchasesAsync();
		await InitializeElectricityPaymentsAsync();
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

	private async Task InitializeEthPayoutsAsync()
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

	private async Task InitializeRolesAsync()
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

	private async Task InitializeUsersAsync()
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

	private async Task InitializeDepositsAsync()
	{
		if (await _context.Deposits.AnyAsync())
			return;

		var users = await _userManagementService.GetUsersAsync();
		var ivanKulaginId = users.First(user => user.FirstName == "Иван").Id;
		var maksimGrigorievId = users.First(user => user is { FirstName: "Максим", LastName: "Григорьев" }).Id;
		var nikitaRanenkoId = users.First(user => user.FirstName == "Никита").Id;
		var maksimRanenkoId = users.First(user => user is { FirstName: "Максим", LastName: "Раненко" }).Id;
		var egorKonyaevId = users.First(user => user.FirstName == "Егор").Id;
		var filippEvseevId = users.First(user => user.FirstName == "Филипп").Id;
		var maksimGlinkinId = users.First(user => user is { FirstName: "Максим", LastName: "Глинкин" }).Id;

		var deposits = new List<DepositDto>
		{
			new(maksimGrigorievId, 70000, new DateTime(2021, 6, 15, 0, 0, 0, DateTimeKind.Utc)),
			new(nikitaRanenkoId, 94050, new DateTime(2021, 6, 17, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimGrigorievId, 306550, new DateTime(2021, 6, 17, 0, 0, 0, DateTimeKind.Utc)),
			new(egorKonyaevId, 250000, new DateTime(2021, 6, 17, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimGrigorievId, 2500, new DateTime(2021, 6, 21, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimGrigorievId, 1825, new DateTime(2021, 6, 25, 0, 0, 0, DateTimeKind.Utc)),
			new(egorKonyaevId, 1175, new DateTime(2021, 6, 25, 0, 0, 0, DateTimeKind.Utc)),
			new(ivanKulaginId, 91000, new DateTime(2021, 7, 19, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimRanenkoId, 100000, new DateTime(2021, 7, 28, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimGrigorievId, 17000, new DateTime(2021, 7, 28, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimRanenkoId, 277, new DateTime(2021, 7, 30, 0, 0, 0, DateTimeKind.Utc)),
			new(nikitaRanenkoId, 210000, new DateTime(2021, 8, 2, 0, 0, 0, DateTimeKind.Utc)),
			new(egorKonyaevId, 6750, new DateTime(2021, 8, 3, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimGrigorievId, 10130, new DateTime(2021, 8, 3, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimRanenkoId, 10000, new DateTime(2021, 8, 8, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimRanenkoId, 133000, new DateTime(2021, 8, 28, 0, 0, 0, DateTimeKind.Utc)),
			new(nikitaRanenkoId, 133000, new DateTime(2021, 8, 28, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimGlinkinId, 133000, new DateTime(2021, 8, 28, 0, 0, 0, DateTimeKind.Utc)),
			new(egorKonyaevId, 2.28m, new DateTime(2021, 9, 4, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimRanenkoId, 263, new DateTime(2021, 9, 21, 0, 0, 0, DateTimeKind.Utc)),
			new(nikitaRanenkoId, 473, new DateTime(2021, 9, 21, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimGlinkinId, 144, new DateTime(2021, 9, 21, 0, 0, 0, DateTimeKind.Utc)),
			new(egorKonyaevId, 279, new DateTime(2021, 9, 21, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimGrigorievId, 442, new DateTime(2021, 9, 21, 0, 0, 0, DateTimeKind.Utc)),
			new(ivanKulaginId, 99, new DateTime(2021, 9, 21, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimRanenkoId, 1240, new DateTime(2021, 10, 23, 0, 0, 0, DateTimeKind.Utc)),
			new(nikitaRanenkoId, 2225, new DateTime(2021, 10, 23, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimGlinkinId, 678, new DateTime(2021, 10, 23, 0, 0, 0, DateTimeKind.Utc)),
			new(egorKonyaevId, 1314, new DateTime(2021, 10, 23, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimGrigorievId, 2079, new DateTime(2021, 10, 23, 0, 0, 0, DateTimeKind.Utc)),
			new(ivanKulaginId, 464, new DateTime(2021, 10, 23, 0, 0, 0, DateTimeKind.Utc)),
			new(filippEvseevId, 80000, new DateTime(2021, 11, 11, 0, 0, 0, DateTimeKind.Utc)),
			new(egorKonyaevId, 50000, new DateTime(2021, 11, 12, 0, 0, 0, DateTimeKind.Utc)),
			new(filippEvseevId, 562, new DateTime(2021, 11, 23, 0, 0, 0, DateTimeKind.Utc)),
			new(nikitaRanenkoId, 3086, new DateTime(2021, 11, 23, 0, 0, 0, DateTimeKind.Utc)),
			new(egorKonyaevId, 2172, new DateTime(2021, 11, 25, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimGlinkinId, 940, new DateTime(2021, 11, 30, 0, 0, 0, DateTimeKind.Utc)),
			new(ivanKulaginId, 642, new DateTime(2021, 11, 30, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimRanenkoId, 1717, new DateTime(2021, 11, 30, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimGrigorievId, 2881, new DateTime(2021, 12, 22, 0, 0, 0, DateTimeKind.Utc)),
			new(nikitaRanenkoId, 800, new DateTime(2022, 01, 14, 0, 0, 0, DateTimeKind.Utc)),
			new(filippEvseevId, 72000, new DateTime(2022, 02, 08, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimRanenkoId, 60000, new DateTime(2022, 02, 10, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimRanenkoId, 20000, new DateTime(2022, 03, 12, 0, 0, 0, DateTimeKind.Utc)),
			new(nikitaRanenkoId, 3691, new DateTime(2022, 08, 17, 0, 0, 0, DateTimeKind.Utc)),
			new(nikitaRanenkoId, 237, new DateTime(2022, 08, 17, 0, 0, 0, DateTimeKind.Utc)),
			new(ivanKulaginId, 768, new DateTime(2022, 08, 17, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimGrigorievId, 3440, new DateTime(2022, 08, 17, 0, 0, 0, DateTimeKind.Utc)),
			new(egorKonyaevId, 2594, new DateTime(2022, 08, 17, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimGlinkinId, 1122, new DateTime(2022, 08, 17, 0, 0, 0, DateTimeKind.Utc)),
			new(filippEvseevId, 1270, new DateTime(2022, 08, 17, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimGrigorievId, 221, new DateTime(2022, 08, 17, 0, 0, 0, DateTimeKind.Utc)),
			new(egorKonyaevId, 166, new DateTime(2022, 08, 17, 0, 0, 0, DateTimeKind.Utc)),
			new(ivanKulaginId, 49, new DateTime(2022, 08, 30, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimGlinkinId, 72, new DateTime(2022, 08, 30, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimRanenkoId, 174, new DateTime(2022, 08, 30, 0, 0, 0, DateTimeKind.Utc)),
			new(filippEvseevId, 81, new DateTime(2022, 08, 30, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimRanenkoId, 2718, new DateTime(2023, 06, 01, 0, 0, 0, DateTimeKind.Utc)),
			new(nikitaRanenkoId, 2307, new DateTime(2023, 06, 01, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimGrigorievId, 2150, new DateTime(2023, 06, 02, 0, 0, 0, DateTimeKind.Utc)),
			new(egorKonyaevId, 1621, new DateTime(2023, 06, 02, 0, 0, 0, DateTimeKind.Utc)),
			new(ivanKulaginId, 480, new DateTime(2023, 06, 02, 0, 0, 0, DateTimeKind.Utc)),
			new(filippEvseevId, 794, new DateTime(2023, 07, 10, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimRanenkoId, 1698, new DateTime(2023, 09, 08, 0, 0, 0, DateTimeKind.Utc)),
			new(maksimGlinkinId, 701, new DateTime(2023, 12, 03, 0, 0, 0, DateTimeKind.Utc)),
		};

		foreach (var deposit in deposits)
		{
			await _depositService.AddDepositAndRecalculateShares(deposit);
		}
	}

	private async Task InitializePurchasesAsync()
	{
		if (await _context.Purchases.AnyAsync())
			return;
		var purchases = new List<CreatePurchaseDto>
		{
			new("Комплектующие для фермы Pleer.ru", 30880, new DateTime(2021, 06, 16, 0, 0, 0, DateTimeKind.Utc)),
			new("Адаптер для монитора Wildberries", 500, new DateTime(2021, 06, 16, 0, 0, 0, DateTimeKind.Utc)),
			new("Двусторонний скотч", 170, new DateTime(2021, 06, 16, 0, 0, 0, DateTimeKind.Utc)),
			new("Удлинитель Pilot МВидео", 1600, new DateTime(2021, 06, 16, 0, 0, 0, DateTimeKind.Utc)),
			new("Комплектующие для фермы Bitok.shop", 6550, new DateTime(2021, 06, 16, 0, 0, 0, DateTimeKind.Utc)),
			new("NVIDIA RTX 3070 5шт.", 625000, new DateTime(2021, 06, 17, 0, 0, 0, DateTimeKind.Utc)),
			new("Блок питания для фермы 2шт. Citilink", 23600, new DateTime(2021, 06, 17, 0, 0, 0, DateTimeKind.Utc)),
			new("Каркас для фермы", 1000, new DateTime(2021, 06, 17, 0, 0, 0, DateTimeKind.Utc)),
			new("Клавиатура для фермы", 700, new DateTime(2021, 06, 21, 0, 0, 0, DateTimeKind.Utc)),
			new("Ethernet кабель для фермы DNS", 700, new DateTime(2021, 06, 21, 0, 0, 0, DateTimeKind.Utc)),
			new("Переходники 2x6+2pin для фермы Pleer.ru", 1300, new DateTime(2021, 06, 21, 0, 0, 0, DateTimeKind.Utc)),
			new("Сетки на окна на балкон для фермы", 4500, new DateTime(2021, 06, 21, 0, 0, 0, DateTimeKind.Utc)),
			new("Дополнительные кулеры для видеокарт", 3000, new DateTime(2021, 06, 25, 0, 0, 0, DateTimeKind.Utc)),
			new("Леруа", 850, new DateTime(2021, 06, 25, 0, 0, 0, DateTimeKind.Utc)),
			new("Продажа видеокарты Nvidia RTX 3070", -85000, new DateTime(2021, 07, 28, 0, 0, 0, DateTimeKind.Utc)),
			new("Whatsminer M20S 68th", 300000, new DateTime(2021, 07, 30, 0, 0, 0, DateTimeKind.Utc)),
			new("Стабилизатор 5квт", 6260, new DateTime(2021, 07, 30, 0, 0, 0, DateTimeKind.Utc)),
			new("Строительные материалы", 3220, new DateTime(2021, 07, 31, 0, 0, 0, DateTimeKind.Utc)),
			new("Строительные материалы", 425, new DateTime(2021, 08, 01, 0, 0, 0, DateTimeKind.Utc)),
			new("Объединение L3++ (Никита)", 120000, new DateTime(2021, 08, 02, 0, 0, 0, DateTimeKind.Utc)),
			new("Объединение S9i (Никита)", 90000, new DateTime(2021, 08, 02, 0, 0, 0, DateTimeKind.Utc)),
			new("Прокладка силового кабеля на даче", 28663, new DateTime(2021, 08, 11, 0, 0, 0, DateTimeKind.Utc)),
			new("Розетка для майнера, кабель, 2 сетки на окна", 4115,
				new DateTime(2021, 08, 12, 0, 0, 0, DateTimeKind.Utc)),
			new("Пирометр и конденсаторы для вытяжки", 1750, new DateTime(2021, 08, 14, 0, 0, 0, DateTimeKind.Utc)),
			new("Объединение Antminer S17Pro", 400000, new DateTime(2021, 08, 28, 0, 0, 0, DateTimeKind.Utc)),
			new("Коньяк для электрика", 1700, new DateTime(2021, 09, 21, 0, 0, 0, DateTimeKind.Utc)),
			new("Стеллаж для майнеров", 8000, new DateTime(2021, 10, 01, 0, 0, 0, DateTimeKind.Utc)),
			new("Хостинг для базы данных", 186, new DateTime(2021, 10, 22, 0, 0, 0, DateTimeKind.Utc)),
			new("SD карта для прошивки майнеров", 800, new DateTime(2022, 01, 14, 0, 0, 0, DateTimeKind.Utc)),
			new("Стабилизатор Ресанта АСН-10000", 10025, new DateTime(2022, 02, 09, 0, 0, 0, DateTimeKind.Utc)),
			new("Коммутатор и 7 патч-кордов (Ситилинк)", 2869, new DateTime(2022, 02, 09, 0, 0, 0, DateTimeKind.Utc)),
			new("Antminer L3+ 3шт.", 220500, new DateTime(2022, 02, 10, 0, 0, 0, DateTimeKind.Utc)),
			new("Кабель и 3 розетки", 3290, new DateTime(2022, 02, 10, 0, 0, 0, DateTimeKind.Utc)),
			new("Кабели питания для новых асиков 3шт.", 1330, new DateTime(2022, 02, 11, 0, 0, 0, DateTimeKind.Utc)),
			new("Шлейфы для L3+ 10шт.", 1500, new DateTime(2022, 02, 14, 0, 0, 0, DateTimeKind.Utc)),
			new("Кулеры для L3+ 4шт.", 3440, new DateTime(2022, 02, 14, 0, 0, 0, DateTimeKind.Utc)),
			new("Осевой вентилятор Axis-Q 450 4E", 12020, new DateTime(2022, 02, 15, 0, 0, 0, DateTimeKind.Utc)),
			new("ОСБ для перегородок в домик, 8шт.", 6920, new DateTime(2022, 03, 12, 0, 0, 0, DateTimeKind.Utc)),
			new("Установка перегородок и вентилятора", 10000, new DateTime(2022, 04, 02, 0, 0, 0, DateTimeKind.Utc)),
			new("Ремонт S17 + доставка", 15600, new DateTime(2022, 08, 06, 0, 0, 0, DateTimeKind.Utc)),
			new("Доставка майнера из ремонта", 1000, new DateTime(2022, 08, 06, 0, 0, 0, DateTimeKind.Utc)),
			new("Ремонт майнера + такси", 9750, new DateTime(2023, 06, 01, 0, 0, 0, DateTimeKind.Utc))
		};

		foreach (var purchase in purchases)
		{
			await _purchaseService.CreatePurchaseAsync(purchase);
		}
	}

	private async Task InitializeElectricityPaymentsAsync()
	{
		if (await _context.ShareablePayments.AnyAsync(payment => payment.Type == PaymentType.Electricity))
			return;

		var users = await _userManagementService.GetUsersAsync();
		var ivanKulaginId = users.First(user => user.FirstName == "Иван").Id;
		var maksimGrigorievId = users.First(user => user is { FirstName: "Максим", LastName: "Григорьев" }).Id;
		var nikitaRanenkoId = users.First(user => user.FirstName == "Никита").Id;
		var maksimRanenkoId = users.First(user => user is { FirstName: "Максим", LastName: "Раненко" }).Id;
		var egorKonyaevId = users.First(user => user.FirstName == "Егор").Id;
		var filippEvseevId = users.First(user => user.FirstName == "Филипп").Id;
		var maksimGlinkinId = users.First(user => user is { FirstName: "Максим", LastName: "Глинкин" }).Id;

		var rubCurrencyId = await _currencyService.GetIdAsync(CurrencyCode.RUB);

		var electricityPayments = new List<ShareablePayment>
		{
			new()
			{
				Id = Guid.NewGuid(), Amount = 2526, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2021, 07, 24, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 4535, Share = 60.25m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2991, Share = 39.75m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 7168, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2021, 08, 15, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 1993, Share = 27.81m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1863, Share = 25.99m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1178, Share = 16.43m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 416, Share = 5.8m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1111, Share = 15.5m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 24250, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2021, 09, 15, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 6744, Share = 27.81m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 6303, Share = 25.99m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 3984, Share = 16.43m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1407, Share = 5.8m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 3759, Share = 15.5m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2054, Share = 8.47m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 23722, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2021, 10, 15, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 6597, Share = 27.81m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 6165, Share = 25.99m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 3898, Share = 16.43m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1376, Share = 5.8m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 3677, Share = 15.5m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2009, Share = 8.47m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 24104, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2021, 11, 15, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 6503, Share = 26.98m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 6072, Share = 25.19m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4577, Share = 18.99m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1355, Share = 5.62m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 3620, Share = 15.02m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1979, Share = 8.21m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 0, Share = 0m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 24395, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2021, 12, 17, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 6284, Share = 25.76m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 5826, Share = 23.88m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4423, Share = 18.13m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1308, Share = 5.36m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 3498, Share = 14.34m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1913, Share = 7.84m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1144, Share = 4.69m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 23500, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2022, 01, 17, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 6044, Share = 25.72m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 5642, Share = 24.01m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4254, Share = 18.1m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1257, Share = 5.35m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 3363, Share = 14.31m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1840, Share = 7.83m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1100, Share = 4.68m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 30693, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2022, 02, 22, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 7342, Share = 23.92m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 6841, Share = 22.29m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 5156, Share = 16.8m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1525, Share = 4.97m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 5070, Share = 16.52m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2231, Share = 7.27m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2526, Share = 8.23m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 29127, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2022, 03, 23, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 6891, Share = 23.66m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 6423, Share = 22.05m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4844, Share = 16.63m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1433, Share = 4.92m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 5074, Share = 17.42m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2094, Share = 7.19m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2371, Share = 8.14m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 32368, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2022, 04, 24, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 7658, Share = 23.66m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 7137, Share = 22.05m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 5383, Share = 16.63m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1593, Share = 4.92m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 5639, Share = 17.42m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2327, Share = 7.19m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2635, Share = 8.14m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 29249, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2022, 05, 24, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 6920, Share = 23.66m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 6449, Share = 22.05m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4864, Share = 16.63m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1439, Share = 4.92m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 5095, Share = 17.42m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2103, Share = 7.19m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2381, Share = 8.14m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 30536, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2022, 06, 23, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 7225, Share = 23.66m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 6733, Share = 22.05m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 5078, Share = 16.63m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1502, Share = 4.92m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 5319, Share = 17.42m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2196, Share = 7.19m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2486, Share = 8.14m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 27527, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2022, 07, 26, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 6513, Share = 23.66m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 6070, Share = 22.05m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4578, Share = 16.63m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1354, Share = 4.92m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4795, Share = 17.42m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1979, Share = 7.19m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2241, Share = 8.14m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 26558, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2022, 08, 24, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 6294, Share = 23.7m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 5864, Share = 22.08m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4422, Share = 16.65m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1309, Share = 4.93m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4595, Share = 17.3m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1912, Share = 7.2m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2164, Share = 8.15m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 28835, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2022, 09, 23, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 6834, Share = 23.7m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 6367, Share = 22.08m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4801, Share = 16.65m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1422, Share = 4.93m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4988, Share = 17.3m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2076, Share = 7.2m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2350, Share = 8.15m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 29914, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2022, 10, 25, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 7090, Share = 23.7m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 6605, Share = 22.08m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4981, Share = 16.65m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1475, Share = 4.93m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 5175, Share = 17.3m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2154, Share = 7.2m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2438, Share = 8.15m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 28051, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2022, 12, 06, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 6648, Share = 23.7m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 6194, Share = 22.08m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4670, Share = 16.65m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1383, Share = 4.93m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4853, Share = 17.3m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2020, Share = 7.2m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2286, Share = 8.15m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 33740, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2022, 12, 30, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 7996, Share = 23.7m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 7450, Share = 22.08m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 5618, Share = 16.65m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1663, Share = 4.93m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 5837, Share = 17.3m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2429, Share = 7.2m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2750, Share = 8.15m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 28636, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2023, 01, 29, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 6787, Share = 23.7m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 6323, Share = 22.08m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4768, Share = 16.65m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1412, Share = 4.93m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4954, Share = 17.3m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2062, Share = 7.2m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2334, Share = 8.15m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 21220, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2023, 02, 24, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 5029, Share = 23.7m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4685, Share = 22.08m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 3533, Share = 16.65m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1046, Share = 4.93m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 3671, Share = 17.3m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1528, Share = 7.2m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1729, Share = 8.15m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 14455, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2023, 03, 22, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 3426, Share = 23.7m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 3192, Share = 22.08m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2407, Share = 16.65m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 713, Share = 4.93m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2501, Share = 17.3m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1041, Share = 7.2m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1178, Share = 8.15m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 6368, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2023, 05, 05, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 1509, Share = 23.7m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1406, Share = 22.08m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1060, Share = 16.65m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 314, Share = 4.93m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1102, Share = 17.3m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 458, Share = 7.2m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 519, Share = 8.15m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 1365, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2023, 05, 05, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 324, Share = 23.7m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 301, Share = 22.08m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 227, Share = 16.65m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 67, Share = 4.93m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 236, Share = 17.3m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 98, Share = 7.2m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 111, Share = 8.15m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 8405, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2023, 05, 24, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 1992, Share = 23.7m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1857, Share = 22.09m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1399, Share = 16.65m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 414, Share = 4.93m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1459, Share = 17.36m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 602, Share = 7.16m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 682, Share = 8.11m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 4845, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2023, 07, 05, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 1148, Share = 23.7m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1070, Share = 22.09m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 807, Share = 16.65m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 239, Share = 4.93m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 841, Share = 17.36m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 347, Share = 7.16m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 393, Share = 8.11m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 12760, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2023, 08, 05, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 3023, Share = 23.69m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2817, Share = 22.08m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2125, Share = 16.65m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 628, Share = 4.92m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2214, Share = 17.35m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 914, Share = 7.16m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1040, Share = 8.15m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 9093, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2023, 09, 05, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 2154, Share = 23.69m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2008, Share = 22.08m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1514, Share = 16.65m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 447, Share = 4.92m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1578, Share = 17.35m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 651, Share = 7.16m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 741, Share = 8.15m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 24726, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2023, 10, 05, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 5853, Share = 23.67m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 5455, Share = 22.06m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4112, Share = 16.63m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1217, Share = 4.92m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4307, Share = 17.42m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1768, Share = 7.15m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2013, Share = 8.14m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 28458, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2023, 12, 01, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 6736, Share = 23.67m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 6278, Share = 22.06m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4733, Share = 16.63m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1400, Share = 4.92m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4957, Share = 17.42m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2035, Share = 7.15m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2316, Share = 8.14m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 16085, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2023, 12, 29, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 3806, Share = 23.66m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 3547, Share = 22.05m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2675, Share = 16.63m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 791, Share = 4.92m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2802, Share = 17.42m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1157, Share = 7.19m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1309, Share = 8.14m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 7611, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2023, 12, 30, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 1801, Share = 23.66m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1678, Share = 22.05m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1266, Share = 16.63m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 374, Share = 4.92m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1326, Share = 17.42m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 547, Share = 7.19m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 620, Share = 8.14m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 6456, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2024, 02, 05, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 1527, Share = 23.66m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1424, Share = 22.05m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1074, Share = 16.63m, UserId = egorKonyaevId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 318, Share = 4.92m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1125, Share = 17.42m, UserId = maksimRanenkoId,
						Status = ShareStatus.Created,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 464, Share = 7.19m, UserId = maksimGlinkinId,
						Status = ShareStatus.Created,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 526, Share = 8.14m, UserId = filippEvseevId,
						Status = ShareStatus.Completed,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 22598, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2024, 03, 05, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 5347, Share = 23.66m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 4983, Share = 22.05m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 3758, Share = 16.63m, UserId = egorKonyaevId,
						Status = ShareStatus.Created,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1112, Share = 4.92m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 3937, Share = 17.42m, UserId = maksimRanenkoId,
						Status = ShareStatus.Created,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1625, Share = 7.19m, UserId = maksimGlinkinId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1839, Share = 8.14m, UserId = filippEvseevId,
						Status = ShareStatus.Created,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 8972, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2024, 04, 05, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 2123, Share = 23.66m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1978, Share = 22.05m, UserId = maksimGrigorievId,
						Status = ShareStatus.Created,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1492, Share = 16.63m, UserId = egorKonyaevId,
						Status = ShareStatus.Created,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 441, Share = 4.92m, UserId = ivanKulaginId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1563, Share = 17.42m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 645, Share = 7.19m, UserId = maksimGlinkinId,
						Status = ShareStatus.Created,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 730, Share = 8.14m, UserId = filippEvseevId,
						Status = ShareStatus.Created,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 10298, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2024, 05, 05, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 2437, Share = 23.66m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2271, Share = 22.05m, UserId = maksimGrigorievId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1713, Share = 16.63m, UserId = egorKonyaevId,
						Status = ShareStatus.Created,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 507, Share = 4.92m, UserId = ivanKulaginId,
						Status = ShareStatus.Created,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1794, Share = 17.42m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 740, Share = 7.19m, UserId = maksimGlinkinId,
						Status = ShareStatus.Created,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 838, Share = 8.14m, UserId = filippEvseevId,
						Status = ShareStatus.Created,
					},
				}
			},
			new()
			{
				Id = Guid.NewGuid(), Amount = 13748, CurrencyId = rubCurrencyId, Type = PaymentType.Electricity,
				Date = new DateTime(2024, 06, 05, 0, 0, 0, DateTimeKind.Utc),
				PaymentShares = new List<PaymentShare>
				{
					new()
					{
						Id = Guid.NewGuid(), Amount = 3253, Share = 23.66m, UserId = nikitaRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 3031, Share = 22.05m, UserId = maksimGrigorievId,
						Status = ShareStatus.Created,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2286, Share = 16.63m, UserId = egorKonyaevId,
						Status = ShareStatus.Created,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 676, Share = 4.92m, UserId = ivanKulaginId,
						Status = ShareStatus.Created,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 2395, Share = 17.42m, UserId = maksimRanenkoId,
						Status = ShareStatus.Completed,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 988, Share = 7.19m, UserId = maksimGlinkinId,
						Status = ShareStatus.Created,
					},
					new()
					{
						Id = Guid.NewGuid(), Amount = 1119, Share = 8.14m, UserId = filippEvseevId,
						Status = ShareStatus.Created,
					},
				}
			},
		};
		await _context.ShareablePayments.AddRangeAsync(electricityPayments);
		await _context.SaveChangesAsync();
	}
}