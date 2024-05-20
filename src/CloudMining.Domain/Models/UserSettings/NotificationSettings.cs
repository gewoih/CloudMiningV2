using CloudMining.Domain.Models.Base;

namespace CloudMining.Domain.Models.UserSettings;

public sealed class NotificationSettings : Entity
{
	public Guid UserId { get; set; }
	public bool NewPayoutNotification { get; set; }
	public bool NewElectricityPaymentNotification { get; set; }
	public bool NewPurchaseNotification { get; set; }
	public bool UnpaidElectricityPaymentReminder { get; set; }
	public bool UnpaidPurchasePaymentReminder { get; set; }
}