namespace CloudMining.Application.Models.Payments.Electricity
{
    public sealed class CreateElectricityPaymentDto
    {
        public DateTime CreatedDate { get; set; }
        public decimal Amount { get; set; }
    }
}
