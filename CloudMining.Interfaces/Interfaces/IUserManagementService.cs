﻿using CloudMining.Domain.Models.Identity;
using CloudMining.Interfaces.DTO.Users;

namespace CloudMining.Interfaces.Interfaces;

public interface IUserManagementService
{
	Task<List<User>> GetMembersAsync();
	Task<User?> GetAsync(Guid userId);
	Task<bool> UpdateAsync(User user);
	Task<User?> GetByTelegramUsernameAsync(string telegramUsername);
	Task<bool> UpdateUserSettingsAsync(Guid userId, UserSettingsDto settings);
}