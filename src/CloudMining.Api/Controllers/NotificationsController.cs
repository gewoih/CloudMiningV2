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
	private readonly ICurrentUserService _currentUserService;
	private readonly IMapper<NotificationSettings, NotificationSettingsDto> _notificationSettingsMapper;

	public NotificationsController(INotificationSettingsService notificationSettingsService, 
		IMapper<NotificationSettings, NotificationSettingsDto> notificationSettingsMapper, 
		ICurrentUserService currentUserService)
	{
		_notificationSettingsService = notificationSettingsService;
		_notificationSettingsMapper = notificationSettingsMapper;
		_currentUserService = currentUserService;
	}

	[HttpGet("settings")]
	public async Task<IActionResult> GetSettings()
	{
		var currentUserId = _currentUserService.GetCurrentUserId();
		if (!currentUserId.HasValue)
			return Forbid();
		
		var notificationSettings = await _notificationSettingsService.GetUserSettingsAsync(currentUserId.Value);
		return Ok(_notificationSettingsMapper.ToDto(notificationSettings));
	}
}