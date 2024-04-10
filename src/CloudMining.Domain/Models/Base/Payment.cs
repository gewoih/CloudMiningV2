namespace CloudMining.Domain.Models.Base
{
	public abstract class Payment : Entity
    {
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
    }
}
