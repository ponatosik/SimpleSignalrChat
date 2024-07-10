using SimpleSignalrChat.DataAccess.Entities;
using SimpleSignalrChat.DataAccess.Exceptions;
using SimpleSignalrChat.DataAccess.Repositories;
using SimpleSignalrChat.DataAccess.Repositories.Interfaces;

namespace SimpleSignalrChat.Test.DataAccess.Repositories;

public class UserRepositoryTest : RepositoryTest
{
	private readonly IUserRepository _userRepository;

	public UserRepositoryTest() : base() 
	{
		_userRepository = new UserRepository(DbContext);
	}

    [Fact]
	public async Task GetUserAsync_UserExists_ShouldReturnUser() 
	{
		var user = new User { Name = "Test" };
		DbContext.Users.Add(user);
		DbContext.SaveChanges();

		var result = await _userRepository.GetUserAsync(1);

		Assert.NotNull(result);
		Assert.Equal(1, result.Id);
		Assert.Equal(user.Name, result.Name);
	}

	[Fact]
	public async Task GetUserAsync_UserDoesntExist_ShouldReturnNull() 
	{
		var user = new User { Name = "Test" };
		DbContext.Users.Add(user);
		DbContext.SaveChanges();

		var result = await _userRepository.GetUserAsync(-999);

		Assert.Null(result);
	}

	[Fact]
	public async Task AddUserAsync_ValidUser_ShouldReturnUser() 
	{
		var user = new User { Name = "Valid user" };

		var result = await _userRepository.AddUserAsync(user);

		Assert.NotNull(result);
		Assert.Equal(user.Name, result.Name);
	}

	[Fact]
	public async Task AddUserAsync_ValidUser_ShouldPersistUser() 
	{
		var user = new User { Name = "Valid user" };

		var result = await _userRepository.AddUserAsync(user);

		Assert.True(DbContext.Users.Any(u => u.Id == result.Id && u.Name == result.Name));
	}

	[Fact]
	public async Task DeleteUserAsync_UserExists_ShouldDeleteUser() 
	{
		var user = new User { Name = "Test" };
		DbContext.Users.Add(user);
		DbContext.SaveChanges();

		await _userRepository.DeleteUserAsync(1);

		Assert.True(!DbContext.Users.Any(u => u.Id == user.Id && u.Name == user.Name));
	}

	[Fact]
	public async Task DeleteUserAsync_UserDoesntExist_ShouldThrowEntityNotFound() 
	{
		var user = new User { Name = "Test" };
		DbContext.Users.Add(user);
		DbContext.SaveChanges();

		await Assert.ThrowsAsync<EntityNotFoundException<User>>(() => _userRepository.DeleteUserAsync(999));
	}
}
