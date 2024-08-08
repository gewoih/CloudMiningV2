using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Modules.Users.Domain.Models;

namespace Modules.Users.Infrastructure.Database;

public sealed class UsersContext : IdentityDbContext<User, Role, Guid>
{
	public UsersContext(DbContextOptions<UsersContext> options) : base(options)
	{
	}
}