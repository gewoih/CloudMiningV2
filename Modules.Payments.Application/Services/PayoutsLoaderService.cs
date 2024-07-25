using CloudMining.Common.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Modules.Currencies.Domain.Enums;
using Modules.Payments.Contracts.DTO;
using Modules.Payments.Contracts.Interfaces;
using Modules.Payments.Domain.Enums;
using Modules.Payments.Infrastructure.Emcd;
using Modules.Payments.Infrastructure.Settings;

namespace Modules.Payments.Application.Services;

public sealed class PayoutsLoaderService : BackgroundService
{
	private readonly int _delayInMinutes;
	private readonly EmcdApiClient _emcdApiClient;
	private readonly IServiceScopeFactory _scopeFactory;

	public PayoutsLoaderService(IServiceScopeFactory scopeFactory, EmcdApiClient emcdApiClient,
		IOptions<PayoutsLoaderSettings> settings)
	{
		_emcdApiClient = emcdApiClient;
		_scopeFactory = scopeFactory;
		_delayInMinutes = settings.Value.DelayInMinutes;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await using var scope = _scopeFactory.CreateAsyncScope();
			var shareablePaymentService = scope.ServiceProvider.GetRequiredService<IShareablePaymentService>();
			await LoadNewPayouts(shareablePaymentService);

			await Task.Delay(TimeSpan.FromMinutes(_delayInMinutes), stoppingToken);
		}
	}

	private async Task LoadNewPayouts(IShareablePaymentService shareablePaymentService)
	{
		var latestCryptoPaymentDate = await shareablePaymentService.GetLatestPaymentDateAsync(PaymentType.Crypto);
		var latestPayouts = await _emcdApiClient.GetPayouts(latestCryptoPaymentDate);

		foreach (var payout in latestPayouts.Take(10))
		{
			var createPaymentDto = new CreatePaymentDto(
				null,
				Enum.Parse<CurrencyCode>(payout.CoinName),
				PaymentType.Crypto,
				payout.GmtTime,
				payout.Amount);

			_ = await shareablePaymentService.CreateAsync(createPaymentDto);
		}
	}
}