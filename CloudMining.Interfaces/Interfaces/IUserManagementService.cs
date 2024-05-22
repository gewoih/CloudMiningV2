using CloudMining.Domain.Models.Identity;

namespace CloudMining.Interfaces.Interfaces
{
	public interface IUserManagementService
	{
        Task<User?> GetAsync(Guid userId);
	}
}
