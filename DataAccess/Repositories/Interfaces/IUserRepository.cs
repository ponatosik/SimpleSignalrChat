using SimpleSignalrChat.DataAccess.Entities;

namespace SimpleSignalrChat.DataAccess.Repositories.Interfaces;

public interface IUserRepository
{
	public Task<User?> GetUserAsync(int id);
	public Task<User?> AddUserAsync(User user);
	public Task DeleteUserAsync(int id);
}
