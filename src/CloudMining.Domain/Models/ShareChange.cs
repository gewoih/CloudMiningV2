﻿using CloudMining.Domain.Models.Base;
using CloudMining.Domain.Models.Identity;

namespace CloudMining.Domain.Models
{
	public sealed class ShareChange : Entity
	{
		public User User { get; set; }
		public Guid UserId { get; set; }
		public Deposit Deposit { get; set; }
		public Guid DepositId { get; set; }
		public DateTime Date { get; set; }
		public decimal Before { get; set; }
		public decimal After { get; set; }
		public decimal Change => After - Before;
	}
}
