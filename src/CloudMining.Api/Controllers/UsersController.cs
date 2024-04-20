using CloudMining.Application.DTO.Users;
using CloudMining.Application.Services.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

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
		public async Task<SignInResult> Login([FromBody] LoginDto credentials)
		{
			var authResult = await _userService.LoginAsync(credentials);
			return authResult;
		}
	}
}
