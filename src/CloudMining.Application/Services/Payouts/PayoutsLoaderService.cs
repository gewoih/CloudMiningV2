using CloudMining.Application.DTO.Payments;
using CloudMining.Application.Services.Payments;
using CloudMining.Domain.Enums;
using CloudMining.Infrastructure.Emcd;
using CloudMining.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace CloudMining.Application.Services.Payouts
{
    public sealed class PayoutsLoaderService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly EmcdApiClient _emcdApiClient;
        private readonly int _delayInMinutes;

        public PayoutsLoaderService(IServiceScopeFactory scopeFactory, EmcdApiClient emcdApiClient, IOptions<PayoutsLoaderSettings> settings)
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
            var latestPayouts = await _emcdApiClient.GetPayouts(fromDate: latestCryptoPaymentDate);

            foreach (var payout in latestPayouts.Take(10))
            {
                var createPaymentDto = new CreatePaymentDto(
                    caption: null,
                    currencyCode: Enum.Parse<CurrencyCode>(payout.CoinName),
                    paymentType: PaymentType.Crypto,
                    dateTime: payout.GmtTime,
                    amount: payout.Amount);

                _ = await shareablePaymentService.CreateAsync(createPaymentDto);
            }
        }
    }
}
