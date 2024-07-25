using CloudMining.Common.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Notifications.Contracts.DTO;
using Modules.Notifications.Contracts.Interfaces;
using Modules.Notifications.Domain.Models;
using Modules.Users.Contracts.Interfaces;

namespace Modules.Notifications.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NotificationsController : ControllerBase
{
	private readonly ICurrentUserService _currentUserService;
	private readonly IMapper<NotificationSettings, NotificationSettingsDto> _notificationSettingsMapper;
	private readonly INotificationSettingsService _notificationSettingsService;

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

	[HttpPatch("settings")]
	public async Task<IActionResult> UpdateSettings([FromBody] NotificationSettingsDto notificationSettingsDto)
	{
		var currentUserId = _currentUserService.GetCurrentUserId();
		var isUpdated =
			await _notificationSettingsService.UpdateUserSettingsAsync(currentUserId.Value, notificationSettingsDto);

		return Ok(isUpdated);
	}
}