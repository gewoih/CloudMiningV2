using CloudMining.Domain.Models;

namespace CloudMining.Application.Services.Payments.Share
{
	public interface IPaymentShareService
	{
		Task<List<PaymentShare>> CreatePaymentShares(decimal amount, Currency currency, DateTime date);
	}
}
