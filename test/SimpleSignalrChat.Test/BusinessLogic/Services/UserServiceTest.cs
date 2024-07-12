using Moq;
using SimpleSignalrChat.BusinessLogic.Services;
using SimpleSignalrChat.BusinessLogic.Services.Interfaces;
using SimpleSignalrChat.DataAccess.Entities;
using SimpleSignalrChat.DataAccess.Repositories.Interfaces;

namespace SimpleSignalrChat.Test.BusinessLogic.Services;

public class UserServiceTest
{
	private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly IUserService _userService;

    public UserServiceTest()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    }

	[Fact]
	public async Task AddUser_WithValidUser_ReturnsUserWithName()
	{
		var userName = "testUser";
		_userRepositoryMock
			.Setup(x => x.AddUserAsync(It.IsAny<User>()))
			.ReturnsAsync((User user) => user);

		var result = await _userService.CreateUserAsync(userName);

		Assert.True(result.IsSuccess);
		Assert.Equal(userName, result.Value!.Name);
	}
	[Fact]
	public async Task GetUser_UserExists_ReturnsUser()
	{
		var user = new User { Id = 1, Name = "testUser" };
		_userRepositoryMock
			.Setup(x => x.GetUserAsync(It.IsAny<int>()))
			.ReturnsAsync(user);

		var result = await _userService.GetUserAsync(1);

		Assert.True(result.IsSuccess);
		Assert.Equal(user.Name, result.Value!.Name);
	}

	[Fact]
	public async Task DeleteUser_UserExists_ReturnsSuccess()
	{
		int userId = 1;
		_userRepositoryMock
			.Setup(x => x.DeleteUserAsync(It.IsAny<int>()));

		var result = await _userService.DeleteUserAsync(userId);

		Assert.True(result.IsSuccess);
	}
}
