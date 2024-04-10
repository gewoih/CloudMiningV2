namespace CloudMining.Models
{
    public abstract class Payment : Entity
    {
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
    }
}
