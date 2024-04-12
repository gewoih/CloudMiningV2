namespace CloudMining.Application.DTO.Payments.Electricity
{
	public sealed class CreateElectricityPaymentDto
	{
		public DateTime Date { get; set; }
		public decimal Amount { get; set; }
	}
}
