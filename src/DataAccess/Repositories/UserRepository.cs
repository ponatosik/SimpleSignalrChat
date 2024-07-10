using Microsoft.EntityFrameworkCore;
using SimpleSignalrChat.DataAccess.Entities;
using SimpleSignalrChat.DataAccess.Exceptions;
using SimpleSignalrChat.DataAccess.Repositories.Interfaces;

namespace SimpleSignalrChat.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
	private readonly ChatContext _chatContext;

	public UserRepository(ChatContext chatContext)
	{
		_chatContext = chatContext;
	}

	public async Task<User?> AddUserAsync(User user)
	{
		_chatContext.Users.Add(user);
		await _chatContext.SaveChangesAsync();
		return user;
	}

	public Task DeleteUserAsync(int id)
	{
		User? user = _chatContext.Users.Find(id);
		if (user is null)
		{
			return Task.FromException(new EntityNotFoundException<User>(id));
		}

		_chatContext.Users.Remove(user);
		return _chatContext.SaveChangesAsync();
	}

	public Task<User?> GetUserAsync(int id)
	{
		return _chatContext.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(user => user.Id == id);
	}
}
