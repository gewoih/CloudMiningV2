namespace CloudMining.Interfaces.DTO
{
    public sealed class UserCalculatedShare
    {
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public decimal Share { get; set; }

        public UserCalculatedShare(Guid userId, decimal amount, decimal share)
        {
            UserId = userId;
            Amount = amount;
            Share = share;
        }
    }
}
