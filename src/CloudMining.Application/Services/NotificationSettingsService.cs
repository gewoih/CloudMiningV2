using CloudMining.Application.Mappings;
using CloudMining.Domain.Models.UserSettings;
using CloudMining.Infrastructure.Database;
using CloudMining.Interfaces.DTO.NotificationSettings;
using CloudMining.Interfaces.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Application.Services;

public sealed class NotificationSettingsService : INotificationSettingsService
{
	private readonly IUserService _userService;
	private readonly CloudMiningContext _context;

	public NotificationSettingsService(IUserService userService, 
		CloudMiningContext context)
	{
		_userService = userService;
		_context = context;
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
}