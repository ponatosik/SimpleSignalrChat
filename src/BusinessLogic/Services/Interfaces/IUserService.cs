using SimpleSignalrChat.BusinessLogic.Abstractions;
using SimpleSignalrChat.BusinessLogic.DTOs;

namespace SimpleSignalrChat.BusinessLogic.Services.Interfaces;

public interface IUserService
{
	public Task<Result<UserDto>> GetUserAsync(int id);
	public Task<Result<UserDto>> CreateUserAsync(string name);
	public Task<Result> DeleteUserAsync(int id);
}
