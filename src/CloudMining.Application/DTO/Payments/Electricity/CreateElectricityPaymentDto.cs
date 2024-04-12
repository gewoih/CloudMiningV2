namespace CloudMining.Application.DTO.Payments.Electricity
{
	public sealed class CreateElectricityPaymentDto
	{
		public DateTime CreatedDate { get; set; }
		public decimal Amount { get; set; }
	}
}
