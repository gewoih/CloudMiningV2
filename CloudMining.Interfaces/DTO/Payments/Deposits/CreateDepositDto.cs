namespace CloudMining.Contracts.DTO.Payments.Deposits
{
	public sealed class CreateDepositDto
	{
		public Guid UserId { get; set; }
		public decimal Amount { get; set; }
		public DateTime Date { get; set; }
	}
}
