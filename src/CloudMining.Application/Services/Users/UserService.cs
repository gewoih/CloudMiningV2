using System.IdentityModel.Tokens.Jwt;
using CloudMining.Application.DTO.Users;
using CloudMining.Application.Services.JWT;
using CloudMining.Domain.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Application.Services.Users
{
	public sealed class UserService : IUserService
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly JwtService _jwtService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public UserService(UserManager<User> userManager, 
			SignInManager<User> signInManager, 
			JwtService jwtService, 
			IHttpContextAccessor httpContextAccessor)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_httpContextAccessor = httpContextAccessor;
			_jwtService = jwtService;
		}

		public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
		{
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
			var authResult = await _signInManager.PasswordSignInAsync(credentials.Email, credentials.Password, true, false);
			var jwt = string.Empty;
			
			if (authResult.Succeeded)
			{
				var user = await _userManager.FindByEmailAsync(credentials.Email);
				jwt = _jwtService.Generate(user);
			}

			return jwt;
		}

        public async Task<List<Guid>> GetAllUsersIdsAsync()
        {
            var usersIds = await _userManager.Users
	            .Select(u => u.Id)
	            .ToListAsync()
	            .ConfigureAwait(false);

            return usersIds;
        }

        public Guid? GetCurrentUserId()
        {
	        var authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
	        if (string.IsNullOrEmpty(authHeader))
		        return null;
	        
	        var jwt = authHeader.Split(' ')[1];
	        var subClaim = _jwtService.GetSubClaim(jwt);
	        return Guid.Parse(subClaim);
        }
	}
}
