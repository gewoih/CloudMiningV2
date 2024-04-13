namespace CloudMining.Application.Models.Payments.Deposits
{
	public sealed class UserDeposits
	{
		public Guid UserId { get; set; }
		public decimal Amount { get; set; }
	
		public UserDeposits(Guid userId, decimal amount)
		{
			UserId = userId;
			Amount = amount;
		}
	}
}
