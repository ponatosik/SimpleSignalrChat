using SimpleSignalrChat.BusinessLogic.Abstractions;
using SimpleSignalrChat.DataAccess.Entities;

namespace SimpleSignalrChat.BusinessLogic.Services.Interfaces;

public interface IUserService
{
	public Task<Result<User>> GetUserAsync(int id);
	public Task<Result<User>> CreateUserAsync(string name);
	public Task<Result> DeleteUserAsync(int id);
}
