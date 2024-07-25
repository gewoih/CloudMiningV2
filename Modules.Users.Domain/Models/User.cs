using Microsoft.AspNetCore.Identity;
using Modules.Notifications.Domain.Models;
using Modules.Payments.Domain.Models;

namespace Modules.Users.Domain.Models;

public class User : IdentityUser<Guid>
{
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Patronymic { get; set; }
	public DateTime? RegistrationDate { get; set; }
	public string? AvatarPath { get; set; }
	public string? TelegramUsername { get; set; }
	public long? TelegramChatId { get; set; }
	public List<Deposit> Deposits { get; set; }
	public List<ShareChange> ShareChanges { get; set; }
	public NotificationSettings NotificationSettings { get; set; }
}