using CloudMining.Application.Models;
using CloudMining.Application.Services.ShareChanges;
using CloudMining.Domain.Models;

namespace CloudMining.Application.Services.Payments.Share
{
	public sealed class PaymentShareService : IPaymentShareService
	{
		private readonly IUserShareService _userShareService;

		public PaymentShareService( IUserShareService userShareService)
		{
			_userShareService = userShareService;
		}

		public async Task<List<PaymentShare>> CreatePaymentShares(decimal amount, Currency currency, DateTime date)
		{
			var usersShares = await CalculateUsersSharesAsync(amount, currency);

			var paymentShares = new List<PaymentShare>();
			foreach (var userShare in usersShares)
			{
				var paymentShare = new PaymentShare
				{
					UserId = userShare.UserId,
					Amount = userShare.Amount,
					Share = userShare.Share,
					CreatedDate = date
				};

				paymentShares.Add(paymentShare);
			}

			return paymentShares;
		}

		private async Task<List<UserCalculatedShare>> CalculateUsersSharesAsync(decimal amount, Currency currency)
		{
			var usersShares = await _userShareService.GetUsersSharesAsync();
			
			var usersCalculatedShares = new List<UserCalculatedShare>();
			foreach (var userShare in usersShares)
			{
				var calculatedAmount =
					Math.Round(userShare.Share * amount, currency.Precision, MidpointRounding.ToZero);

				usersCalculatedShares.Add(new UserCalculatedShare(userShare.UserId, calculatedAmount, userShare.Share));
			}

			return usersCalculatedShares;
		}
	}
}
