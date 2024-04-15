using System.ComponentModel.DataAnnotations;

namespace CloudMining.Application.DTO.Payments.Deposits
{
	public sealed class CreateDepositDto
	{
		public Guid UserId { get; set; }
		public Guid CurrencyId { get; set; }
		
		[Range(1, 1_000_000)]
		public decimal Amount { get; set; }
		
		public DateTime Date { get; set; }
	}
}
