using CloudMining.Application.Mappings;
using CloudMining.Domain.Models.UserSettings;
using CloudMining.Infrastructure.Database;
using CloudMining.Interfaces.DTO.NotificationSettings;
using CloudMining.Interfaces.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Application.Services;

public sealed class NotificationSettingsService : INotificationSettingsService
{
	private readonly CloudMiningContext _context;
	private readonly ICurrentUserService _currentUserService;
	private readonly IMapper<NotificationSettings, NotificationSettingsDto> _notificationSettingsMapper;

	public NotificationSettingsService(ICurrentUserService currentUserService,
		CloudMiningContext context,
		IMapper<NotificationSettings, NotificationSettingsDto> notificationSettingsMapper)
	{
		_currentUserService = currentUserService;
		_context = context;
		_notificationSettingsMapper = notificationSettingsMapper;
	}

	public async Task<NotificationSettings?> GetUserSettingsAsync(Guid? userId = null)
	{
		var currentUserId = userId ?? _currentUserService.GetCurrentUserId();

		var userSettings = await _context.NotificationSettings.FirstOrDefaultAsync(settings =>
			settings.UserId == currentUserId);

		return userSettings;
	}

	public async Task<NotificationSettings> UpdateSettingsAsync(Guid userId, NotificationSettingsDto notificationSettingsDto)
	{
		var userSettings = await GetUserSettingsAsync(userId);

		if (userSettings is null)
		{
			userSettings = _notificationSettingsMapper.ToDomain(notificationSettingsDto);
			await _context.NotificationSettings.AddAsync(userSettings);
		}
		else
		{
			//TODO: Возможно ли сделать через маппер?
			userSettings.IsTelegramNotificationsEnabled = notificationSettingsDto.IsTelegramNotificationsEnabled;
			userSettings.NewPayoutNotification = notificationSettingsDto.NewPayoutNotification;
			userSettings.NewPurchaseNotification = notificationSettingsDto.NewPurchaseNotification;
			userSettings.NewElectricityPaymentNotification = notificationSettingsDto.NewElectricityPaymentNotification;
			userSettings.UnpaidElectricityPaymentReminder = notificationSettingsDto.UnpaidElectricityPaymentReminder;
			userSettings.UnpaidPurchasePaymentReminder = notificationSettingsDto.UnpaidPurchasePaymentReminder;
		}

		await _context.SaveChangesAsync();
		return userSettings;
	}
}