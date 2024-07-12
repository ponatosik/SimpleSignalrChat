using Moq;
using SimpleSignalrChat.BusinessLogic.Abstractions;
using SimpleSignalrChat.BusinessLogic.DTOs;
using SimpleSignalrChat.BusinessLogic.Services;
using SimpleSignalrChat.BusinessLogic.Services.Interfaces;
using SimpleSignalrChat.DataAccess.Entities;
using SimpleSignalrChat.DataAccess.Repositories.Interfaces;

namespace SimpleSignalrChat.Test.BusinessLogic.Services;

public class ChatServiceTest
{
	private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IChatRepository> _chatRepositoryMock;
    private readonly IChatService _chatService;

    public ChatServiceTest()
    {
        _chatRepositoryMock = new Mock<IChatRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _chatService = new ChatService(_chatRepositoryMock.Object, _userRepositoryMock.Object);

        _userRepositoryMock
			.Setup(x => x.GetUserAsync(It.IsAny<int>()))
			.ReturnsAsync((int id) => new User
			{
				Id = id,
				Name = "testUser"
			});
    }

	[Fact]
	public async Task GetChatAsync_ChatExists_ReturnsChat()
	{
		var admin = new User { Id = 1, Name = "testUser" };
		var chat = new Chat { Id = 1, Name = "testChat", Admin = admin };
		_chatRepositoryMock
			.Setup(x => x.GetChatAsync(It.IsAny<int>()))
			.ReturnsAsync(chat);

		var result = await _chatService.GetChatAsync(1);

		Assert.True(result.IsSuccess);
		Assert.Equal(chat.Name, result.Value!.Name);
	}

	[Fact]
	public async Task GetAllChatsAsync_ChatExists_ReturnsChat()
	{
		var admin = new User { Id = 1, Name = "testUser" };
		var chats = new List<Chat>
		{
			new Chat { Id = 1, Name = "testChat", Admin = admin },
			new Chat { Id = 2, Name = "testChat2", Admin = admin },
			new Chat { Id = 3, Name = "testChat3", Admin = admin }
		};
		_chatRepositoryMock
			.Setup(x => x.GetAllChatsAsync())
			.ReturnsAsync(chats);

		var result = await _chatService.GetAllChatsAsync();

		Assert.True(result.IsSuccess);
		Assert.Collection(result.Value!,
			chat => Assert.Equal(chats[0].Name, chat.Name),
			chat => Assert.Equal(chats[1].Name, chat.Name),
			chat => Assert.Equal(chats[2].Name, chat.Name));
	}

	[Fact]
	public async Task CreateChatAsync_ValidChat_ReturnsChat()
	{
		var admin = new User { Id = 1, Name = "testUser" };
		var chatName = "testChat";
		_chatRepositoryMock
			.Setup(x => x.AddChatAsync(It.IsAny<Chat>()))
			.ReturnsAsync((Chat chat) => chat);

		var result = await _chatService.CreateChatAsync(admin.Id, chatName);

		Assert.True(result.IsSuccess);
		Assert.Equal(chatName, result.Value!.Name);
	}

	[Fact]
	public async Task UpdateChatAsync_ChatAdmin_ReturnsChat()
	{
		var newChatName = "updated";
		var admin = new User { Id = 1, Name = "testUser" };
		var chat = new Chat { Id = 1, Name = "chat", Admin = admin };
		_chatRepositoryMock
			.Setup(x => x.GetChatAsync(It.IsAny<int>()))
			.ReturnsAsync((int id) => chat);
		_chatRepositoryMock
			.Setup(x => x.UpdateChatAsync(It.IsAny<int>(), It.IsAny<Chat>()))
			.ReturnsAsync((int id, Chat chat) => chat);

		var result = await _chatService.UpdateChatAsync(1, newChatName, admin.Id);

		Assert.True(result.IsSuccess);
		Assert.Equal(newChatName, result.Value!.Name);
	}

	[Fact]
	public async Task DeleteChatAsync_ChatAdmin_ReturnsSuccess()
	{
		_chatRepositoryMock
			.Setup(x => x.GetChatAsync(It.IsAny<int>()))
			.ReturnsAsync((int id) => new Chat { Id = id, Name = "chat", Admin = new User { Id = 1, Name = "testUser" } });
		_chatRepositoryMock
			.Setup(x => x.DeleteChatAsync(It.IsAny<int>()))
			.Returns(() => Task.CompletedTask);

		var result = await _chatService.DeleteChatAsync(1, 1);

		Assert.True(result.IsSuccess);
	}
}
