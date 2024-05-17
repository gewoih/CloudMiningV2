using CloudMining.Application.DTO.Users;
using CloudMining.Application.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CloudMining.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserService _userService;

		public UsersController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpPost]
		public async Task<IdentityResult> Register([FromBody] RegisterDto credentials)
		{
			var registrationResult = await _userService.RegisterAsync(credentials);
			return registrationResult;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto credentials)
		{
			var userJwt = await _userService.LoginAsync(credentials);
			if (string.IsNullOrEmpty(userJwt))
				return Unauthorized();
			
			return Ok(userJwt);
		}
	}
}
