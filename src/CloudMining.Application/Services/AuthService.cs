using CloudMining.Domain.Models.Identity;
using CloudMining.Interfaces.DTO.Users;
using CloudMining.Interfaces.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CloudMining.Application.Services;

public sealed class AuthService : IAuthService
{
	private readonly UserManager<User> _userManager;
	private readonly SignInManager<User> _signInManager;
	private readonly JwtService _jwtService;
	private readonly ICurrentUserService _currentUserService;

	public AuthService(UserManager<User> userManager,
		SignInManager<User> signInManager,
		JwtService jwtService,
		ICurrentUserService currentUserService)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_jwtService = jwtService;
		_currentUserService = currentUserService;
	}

	public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
	{
		//TODO: Добавить маппер
		var newUser = new User
		{
			Email = dto.Email,
			UserName = dto.Email,
			FirstName = dto.FirstName,
			LastName = dto.LastName,
			Patronymic = dto.Patronymic
		};

		return await _userManager.CreateAsync(newUser, dto.Password);
	}

	public async Task<string> LoginAsync(LoginDto credentials)
	{
		var authResult =
			await _signInManager.PasswordSignInAsync(credentials.Email, credentials.Password, true, false);
		var jwt = string.Empty;

		if (authResult.Succeeded)
		{
			var user = await _userManager.FindByEmailAsync(credentials.Email);
			if (user is not null)
				jwt = await _jwtService.GenerateAsync(user);
		}

		return jwt;
	}

	public async Task<bool> ChangeEmailAsync(ChangeEmailDto dto)
	{
		var userId = _currentUserService.GetCurrentUserId();
		if (userId == null)
			return false;

		var user = await _userManager.FindByIdAsync(userId.ToString());
		if (user is null)
			return false;

		var token = await _userManager.GenerateChangeEmailTokenAsync(user, dto.Email);
		var result = await _userManager.ChangeEmailAsync(user, dto.Email, token);

		user.UserName = user.Email;
		await _userManager.UpdateAsync(user);

		return result.Succeeded;
	}

	public async Task<bool> ChangePasswordAsync(ChangePasswordDto dto)
	{
		var userId = _currentUserService.GetCurrentUserId();
		if (userId == null)
			return false;

		var user = await _userManager.FindByIdAsync(userId.ToString());
		var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
		return result.Succeeded;
	}
}