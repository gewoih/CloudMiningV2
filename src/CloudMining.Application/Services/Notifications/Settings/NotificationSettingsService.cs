using CloudMining.Application.DTO.NotificationSettings;
using CloudMining.Application.Mappings;
using CloudMining.Application.Services.Users;
using CloudMining.Domain.Models.UserSettings;
using CloudMining.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Application.Services.Notifications.Settings;

public sealed class NotificationSettingsService : INotificationSettingsService
{
	private readonly IUserService _userService;
	private readonly CloudMiningContext _context;
	private readonly IMapper<NotificationSettings, NotificationSettingsDto> _notificationSettingsMapper;
	
	public NotificationSettingsService(IUserService userService, 
		CloudMiningContext context, 
		IMapper<NotificationSettings, NotificationSettingsDto> notificationSettingsMapper)
	{
		_userService = userService;
		_context = context;
		_notificationSettingsMapper = notificationSettingsMapper;
	}

	public async Task<NotificationSettings> GetUserSettingsAsync()
	{
		var currentUserId = _userService.GetCurrentUserId();

		var currentUserSettings = await _context.Users
			.Where(user => user.Id == currentUserId)
			.Select(user => user.NotificationSettings)
			.FirstOrDefaultAsync();

		return currentUserSettings ?? new NotificationSettings();
	}

	public async Task<bool> UpdateUserSettingsAsync(NotificationSettingsDto settingsDto)
	{
		var currentUserId = _userService.GetCurrentUserId();
		if (currentUserId == null)
			return false;

		var currentUserSettings = await _context.NotificationSettings.FirstOrDefaultAsync(settings => 
			settings.UserId == currentUserId);

		if (currentUserSettings is null)
		{
			currentUserSettings = _notificationSettingsMapper.ToDomain(settingsDto);
			await _context.NotificationSettings.AddAsync(currentUserSettings);
		}
		else
		{
			//TODO: Возможно ли сделать через маппер?
			currentUserSettings.NewPayoutNotification = settingsDto.NewPayoutNotification;
			currentUserSettings.NewPurchaseNotification = settingsDto.NewPurchaseNotification;
			currentUserSettings.NewElectricityPaymentNotification = settingsDto.NewElectricityPaymentNotification;
			currentUserSettings.UnpaidElectricityPaymentReminder = settingsDto.UnpaidElectricityPaymentReminder;
			currentUserSettings.UnpaidPurchasePaymentReminder = settingsDto.UnpaidPurchasePaymentReminder;
		}
		
		var updatedRows = await _context.SaveChangesAsync();
		return updatedRows > 0;
	}
}