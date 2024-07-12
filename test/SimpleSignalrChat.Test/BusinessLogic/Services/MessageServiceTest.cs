using Moq;
using SimpleSignalrChat.BusinessLogic.Abstractions;
using SimpleSignalrChat.BusinessLogic.DTOs;
using SimpleSignalrChat.BusinessLogic.Services;
using SimpleSignalrChat.BusinessLogic.Services.Interfaces;
using SimpleSignalrChat.DataAccess.Entities;
using SimpleSignalrChat.DataAccess.Repositories.Interfaces;

namespace SimpleSignalrChat.Test.BusinessLogic.Services;

public class MessageServiceTest
{
	private readonly Mock<IMessageRepository> _messageRepositoryMock;
	private readonly Mock<IUserRepository> _userRepositoryMock;
	private readonly Mock<IChatRepository> _chatRepositoryMock;
	private readonly IMessageService _messageService;

    public MessageServiceTest()
    {
		_messageRepositoryMock = new Mock<IMessageRepository>();
		_userRepositoryMock = new Mock<IUserRepository>();
		_chatRepositoryMock = new Mock<IChatRepository>();

		_messageService = new MessageService(_messageRepositoryMock.Object, _chatRepositoryMock.Object, _userRepositoryMock.Object);

		_userRepositoryMock
			.Setup(x => x.GetUserAsync(It.IsAny<int>()))
			.ReturnsAsync((int id) => new User
			{
				Id = id,
				Name = "testUser"
			});
	}

	[Fact]
	public async Task GetMessageAsync_MessageExists_ReturnsMessage()
	{
		var message = new Message
		{
			Id = 1,
			Sender = new User { Id = 1, Name = "testUser" },
			Chat = new Chat { Id = 1, Name = "testChat" },
			Content = "testMessage"
		};
		_messageRepositoryMock
			.Setup(x => x.GetMessageAsync(It.IsAny<int>()))
			.ReturnsAsync(message);

		var result = await _messageService.GetMessageAsync(1);

		Assert.True(result.IsSuccess);
		Assert.Equal(message.Content, result.Value!.Contet);
	}

	//public Task<Result<List<MessageDto>>> GetChatMessagesAsync(int chatId);
	[Fact]
	public async Task GetChatMessagesAsync_ChatExists_ReturnsMessages()
	{
		var chat = new Chat { Id = 1, Name = "testChat" };
		var messages = new List<Message>
		{
			new Message
			{
				Id = 1,
				Sender = new User { Id = 1, Name = "testUser" },
				Chat = chat,
				Content = "testMessage"
			},
			new Message
			{
				Id = 2,
				Sender = new User { Id = 1, Name = "testUser" },
				Chat = chat,
				Content = "testMessage2"
			}
		};
		_chatRepositoryMock.
			Setup(x => x.GetChatAsync(It.IsAny<int>()))
			.ReturnsAsync(chat);
		_messageRepositoryMock
			.Setup(x => x.GetChatMessagesAsync(It.IsAny<Chat>()))
			.ReturnsAsync(messages);

		var result = await _messageService.GetChatMessagesAsync(1);

		Assert.True(result.IsSuccess);
		Assert.Collection(result.Value!,
			message => Assert.Equal(messages[0].Content, message.Contet),
			message => Assert.Equal(messages[1].Content, message.Contet));
	}

	[Fact]
	public async Task AddMessageAsync_ChatExists_ReturnsMessageInfo()
	{
		var chat = new Chat { Id = 1, Name = "testChat" };
		var messageContent = "test message";
		var message = new Message
		{
			Id = 1,
			Sender = new User { Id = 1, Name = "testUser" },
			Chat = chat,
			Content = messageContent
		};
		_chatRepositoryMock
			.Setup(x => x.GetChatAsync(It.IsAny<int>()))
			.ReturnsAsync(chat);
		_messageRepositoryMock
			.Setup(x => x.AddMessageAsync(It.IsAny<Message>()))
			.ReturnsAsync(message);

		var result = await _messageService.AddMessageAsync(1, 1, messageContent);

		Assert.True(result.IsSuccess);
		Assert.Equal(messageContent, result.Value!.Contet);
	}

	[Fact]
	public async Task DeleteMessageAsync_Sender_ReturnsSuccess()
	{
		var senderId = 1;
		var messageId = 1;
		var message = new Message
		{
			Id = messageId,
			Sender = new User { Id = senderId, Name = "testUser" },
			Chat = new Chat { Id = 1, Name = "testChat" },
			Content = "testMessage"
		};
		_messageRepositoryMock
			.Setup(x => x.GetMessageAsync(It.IsAny<int>()))
			.ReturnsAsync(message);

		var result = await _messageService.DeleteMessageAsync(messageId, senderId);

		Assert.True(result.IsSuccess);
	}

}
