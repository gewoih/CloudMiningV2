namespace CloudMining.Application.Models.Shares
{
    public sealed class UserShare
    {
        public Guid UserId { get; set; }
        public decimal Share { get; set; }

        public UserShare(Guid userId, decimal share)
        {
            UserId = userId;
            Share = share;
        }
	}
}
