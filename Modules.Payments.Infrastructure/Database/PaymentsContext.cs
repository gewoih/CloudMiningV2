using CloudMining.Common.Database;
using Microsoft.EntityFrameworkCore;
using Modules.Payments.Domain.Models;

namespace Modules.Payments.Infrastructure.Database;

public sealed class PaymentsContext : CloudMiningContext
{
	public DbSet<Deposit> Deposits { get; set; }
	public DbSet<PaymentShare> PaymentShares { get; set; }
	public DbSet<ShareablePayment> ShareablePayments { get; set; }
	public DbSet<ShareChange> ShareChanges { get; set; }
	
	
	public PaymentsContext(DbContextOptions<PaymentsContext> options) : base(options)
	{
	}
}