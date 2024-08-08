using CloudMining.Common.Database;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Modules.Notifications.Domain.Models;

namespace Modules.Notifications.Infrastructure.Database;

public sealed class NotificationsContext : CloudMiningContext
{
	public DbSet<NotificationSettings> NotificationSettings { get; set; }
	public DbSet<Notification> Notifications { get; set; }
	public DbSet<OutboxState> OutboxStates { get; set; }
	
	public NotificationsContext(DbContextOptions<NotificationsContext> options) : base(options)
	{
	}
	
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.AddInboxStateEntity();
		modelBuilder.AddOutboxMessageEntity();
		modelBuilder.AddOutboxStateEntity();
	}
}