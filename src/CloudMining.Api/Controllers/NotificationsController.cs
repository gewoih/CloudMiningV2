using CloudMining.Application.Mappings;
using CloudMining.Domain.Models.UserSettings;
using CloudMining.Interfaces.DTO.NotificationSettings;
using CloudMining.Interfaces.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudMining.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NotificationsController : ControllerBase
{
	private readonly INotificationSettingsService _notificationSettingsService;
	private readonly IMapper<NotificationSettings, NotificationSettingsDto> _notificationSettingsMapper;

	public NotificationsController(INotificationSettingsService notificationSettingsService, 
		IMapper<NotificationSettings, NotificationSettingsDto> notificationSettingsMapper)
	{
		_notificationSettingsService = notificationSettingsService;
		_notificationSettingsMapper = notificationSettingsMapper;
	}

	[HttpGet("settings")]
	public async Task<NotificationSettingsDto> GetSettings()
	{
		var notificationSettings = await _notificationSettingsService.GetUserSettingsAsync();
		return _notificationSettingsMapper.ToDto(notificationSettings);
	}

	[HttpPatch("settings")]
	public async Task<bool> UpdateSettings([FromBody] NotificationSettingsDto settings)
	{
		var isUpdated = await _notificationSettingsService.UpdateUserSettingsAsync(settings);
		return isUpdated;
	}
}