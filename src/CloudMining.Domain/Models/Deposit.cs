﻿using CloudMining.Domain.Models.Base;
using CloudMining.Domain.Models.Identity;

namespace CloudMining.Domain.Models
{
	public class Deposit : Payment
	{
		public User User { get; set; }
	}
}