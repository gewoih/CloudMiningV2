using CloudMining.Interfaces.DTO.File;
using CloudMining.Interfaces.DTO.Users;
using CloudMining.Interfaces.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CloudMining.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
	private readonly IAuthService _authService;
	private readonly ICurrentUserService _currentUserService;

	public UsersController(IAuthService authService, ICurrentUserService currentUserService)
	{
		_authService = authService;
		_currentUserService = currentUserService;
	}

	[HttpPost]
	public async Task<IdentityResult> Register([FromBody] RegisterDto credentials)
	{
		var registrationResult = await _authService.RegisterAsync(credentials);
		return registrationResult;
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginDto credentials)
	{
		var userJwt = await _authService.LoginAsync(credentials);
		if (string.IsNullOrEmpty(userJwt))
			return Unauthorized();

		return Ok(userJwt);
	}

	[Authorize]
	[HttpPatch("email")]
	public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto dto)
	{
		var succeeded = await _authService.ChangeEmailAsync(dto);
		if (!succeeded)
			return BadRequest();

		return Ok();
	}

	[Authorize]
	[HttpPatch("password")]
	public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
	{
		var succeeded = await _authService.ChangePasswordAsync(dto);
		if (!succeeded)
			return Unauthorized();

		return Ok();
	}

	[Authorize]
	[HttpPatch("avatar")]
	public async Task<IActionResult> ChangeAvatar([FromForm] FileDto file)
	{
		var newAvatarPath = await _currentUserService.ChangeAvatarAsync(file);
		if (string.IsNullOrEmpty(newAvatarPath))
			return Unauthorized();

		return Ok(newAvatarPath);
	}
}