﻿namespace CloudMining.Interfaces.DTO
{
	public sealed class UserDeposits
	{
		public Guid UserId { get; set; }
		public decimal Amount { get; set; }
	
		public UserDeposits(Guid userId, decimal amount)
		{
			UserId = userId;
			Amount = amount;
		}
	}
}
