using CloudMining.Domain.Models.Identity;
using CloudMining.Infrastructure.Database;
using CloudMining.Interfaces.DTO.Users;
using CloudMining.Interfaces.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Application.Services;

public sealed class UserManagementService : IUserManagementService
{
	private readonly CloudMiningContext _context;
	private readonly INotificationSettingsService _notificationSettingsService;

	public UserManagementService(CloudMiningContext context, INotificationSettingsService notificationSettingsService)
	{
		_context = context;
		_notificationSettingsService = notificationSettingsService;
	}

	public async Task<List<User>> GetUsersAsync(bool withDeposits = false, bool withShareChanges = false)
	{
		var usersQuery = _context.Users.AsQueryable();

		if (withDeposits)
			usersQuery = usersQuery.Include(user => user.Deposits);

		if (withShareChanges)
			usersQuery = usersQuery.Include(user => user.ShareChanges);

		return await usersQuery.ToListAsync().ConfigureAwait(false);
	}

	public async Task<User?> GetAsync(Guid userId)
	{
		return await _context.Users.FindAsync(userId);
	}

	public async Task<bool> UpdateAsync(User user)
	{
		_context.Users.Update(user);

		var updatedRows = await _context.SaveChangesAsync().ConfigureAwait(false);
		return updatedRows > 0;
	}

	public async Task<User?> GetByTelegramUsernameAsync(string telegramUsername)
	{
		return await _context.Users.FirstOrDefaultAsync(user => user.TelegramUsername == telegramUsername);
	}

	public async Task<bool> UpdateUserSettingsAsync(Guid userId, UserSettingsDto settings)
	{
		await using var transaction = await _context.Database.BeginTransactionAsync();
		try
		{
			var user = await GetAsync(userId);
			if (user is null)
				return false;

			if (settings.NotificationSettings is not null)
				await _notificationSettingsService.UpdateUserSettingsAsync(userId, settings.NotificationSettings);

			if (!string.IsNullOrEmpty(settings.TelegramUsername))
				user.TelegramUsername = settings.TelegramUsername;

			await transaction.CommitAsync();
		}
		catch
		{
			await transaction.RollbackAsync();
			return false;
		}

		return true;
	}
}