using CloudMining.Application.DTO.Users;
using CloudMining.Application.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace CloudMining.App.Controllers
{
	[Route("[controller]")]
	public class UserController : Controller
	{
		private readonly IUserService _userService;

		public UserController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpPost]
		public async Task<IActionResult> Register([FromBody] RegisterDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var registrationResult = await _userService.RegisterAsync(dto);
			return View(registrationResult);
		}

		[HttpPost("auth")]
		public async Task<IActionResult> Login([FromBody] LoginDto credentials)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var authResult = await _userService.LoginAsync(credentials);
			return View(authResult);
		}
	}
}
