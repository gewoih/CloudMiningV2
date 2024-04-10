namespace CloudMining.Models
{
    public class PaymentShare : Entity
    {
        public User User { get; set; }
        public decimal Amount { get; set; }
        public decimal Percent { get; set; }
        public bool IsCompleted { get; set; }
    }
}
