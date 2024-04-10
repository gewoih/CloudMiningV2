namespace CloudMining.Models
{
    public class ShareablePayment : Payment
    {
        public PaymentType Type { get; set; }
        public bool IsCompleted { get; set; }
        public List<PaymentShare> PaymentShares { get; set; }
    }
}
