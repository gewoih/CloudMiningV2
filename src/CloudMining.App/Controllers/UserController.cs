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

		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterDto credentials)
		{
			if (!ModelState.IsValid)
				return View(credentials);

			var registrationResult = await _userService.RegisterAsync(credentials);
			if (!registrationResult.Succeeded)
			{
				foreach (var error in registrationResult.Errors)
				{
					ModelState.AddModelError(error.Code, error.Description);
				}
			}

			return View(credentials);
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
