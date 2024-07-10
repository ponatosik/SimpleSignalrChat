using SimpleSignalrChat.DataAccess.Entities;
using SimpleSignalrChat.DataAccess.Exceptions;
using SimpleSignalrChat.DataAccess.Repositories;

namespace SimpleSignalrChat.Test.DataAccess.Repositories;

public class MessageRepositoryTest: RepositoryTest
{
	private readonly MessageRepository _messageRepository;

    public MessageRepositoryTest()
    {
        _messageRepository = new MessageRepository(DbContext);
    }

	[Fact]
	public async Task GetMessageAsync_MessageExists_ShouldReturnMessage()
	{
		var user = new User { Name = "admin" };
		var chat = new Chat { Name = "test", Admin = user };
		var message = new Message { Chat = chat, Sender = user, Content = "test message", SentAt = DateTime.Now };
		DbContext.Users.Add(user);
		DbContext.Chats.Add(chat);
		DbContext.Messages.Add(message);
		DbContext.SaveChanges();

		var result = await _messageRepository.GetMessageAsync(1);
		Assert.NotNull(result);
		Assert.Equal(message.Chat.Id, result.Chat.Id);
		Assert.Equal(message.Sender.Id, result.Sender.Id);
		Assert.Equal(message.Content, result.Content);
		Assert.Equal(message.SentAt, result.SentAt);
	}
	
	[Fact]
	public async Task GetMessageAsync_MessageDosentExist_ShouldReturnNull()
	{
		var user = new User { Name = "admin" };
		var chat = new Chat { Name = "test", Admin = user };
		var message = new Message { Chat = chat, Sender = user, Content = "test", SentAt = DateTime.Now };
		DbContext.Users.Add(user);
		DbContext.Chats.Add(chat);
		DbContext.Messages.Add(message);
		DbContext.SaveChanges();

		var result = await _messageRepository.GetMessageAsync(-999);
		Assert.Null(result);
	}

	[Fact]
	public async Task GetChatMessagesAsync_Allways_ShouldReturnChatMessages()
	{
		var user = new User { Name = "admin" };
		var chat1 = new Chat { Name = "test", Admin = user };
		var chat2 = new Chat { Name = "test", Admin = user };
		var message1 = new Message { Chat = chat1, Sender = user, Content = "test 1", SentAt = DateTime.Now };
		var message2 = new Message { Chat = chat1, Sender = user, Content = "test 2", SentAt = DateTime.Now };
		var message3 = new Message { Chat = chat1, Sender = user, Content = "test 3", SentAt = DateTime.Now };
		var message4 = new Message { Chat = chat2, Sender = user, Content = "test 3", SentAt = DateTime.Now };
		DbContext.Users.Add(user);
		DbContext.Chats.AddRange(chat1, chat2);
		DbContext.Messages.AddRange(message1, message2, message3, message4);
		DbContext.SaveChanges();

		var result = await _messageRepository.GetChatMessagesAsync(new Chat { Id = chat1.Id });

		Assert.NotEmpty(result);
		Assert.Equal(3, result.Count());
		Assert.Contains(result, m => m.Content == message1.Content);
		Assert.Contains(result, m => m.Content == message2.Content);
		Assert.Contains(result, m => m.Content == message3.Content);
	}

	[Fact]
	public async Task AddMessageAsync_ChatExtists_ShouldReturnMessage()
	{
		var user = new User { Name = "admin" };
		var chat = new Chat { Name = "test", Admin = user };
		DbContext.Users.Add(user);
		DbContext.Chats.Add(chat);
		DbContext.SaveChanges();
		
		var message = new Message { Chat = chat, Sender = user, Content = "test", SentAt = DateTime.Now };

		var result = await _messageRepository.AddMessageAsync(message);
		Assert.NotNull(result);
		Assert.Equal(message.Chat.Id, result.Chat.Id);
		Assert.Equal(message.Sender.Id, result.Sender.Id);
		Assert.Equal(message.Content, result.Content);
		Assert.Equal(message.SentAt, result.SentAt);
	}

	[Fact]
	public async Task AddMessageAsync_ChatExtists_ShouldPersistMessage()
	{
		var user = new User { Name = "admin" };
		var chat = new Chat { Name = "test", Admin = user };
		DbContext.Users.Add(user);
		DbContext.Chats.Add(chat);
		DbContext.SaveChanges();
		
		var newMessage = new Message { Chat = chat, Sender = user, Content = "test", SentAt = DateTime.Now };
		await _messageRepository.AddMessageAsync(newMessage);

		Assert.True(DbContext.Messages.Any(
			message => message.Id == newMessage.Id &&
			message.Content == newMessage.Content &&
			message.SentAt == newMessage.SentAt &&
			message.Chat.Id == newMessage.Chat.Id &&
			message.Sender.Id == newMessage.Sender.Id));
	}

	[Fact]
	public async Task AddMessageAsync_ChatDoesntExtist_ShouldThrowEntityNotFoundException()
	{
		var user = new User { Name = "admin" };
		var chat = new Chat { Name = "not id database", Admin = user };
		DbContext.Users.Add(user);
		DbContext.SaveChanges();
		
		var newMessage = new Message { Chat = chat, Sender = user, Content = "test", SentAt = DateTime.Now };
		await Assert.ThrowsAsync<EntityNotFoundException<Chat>>(() => _messageRepository.AddMessageAsync(newMessage));
	}

	[Fact]
	public async Task DeleteMessageAsync_MessageExists_ShouldDeleteMessage()
	{
		var user = new User { Name = "admin" };
		var chat = new Chat { Name = "test", Admin = user };
		var message = new Message { Chat = chat, Sender = user, Content = "test", SentAt = DateTime.Now };
		DbContext.Users.Add(user);
		DbContext.Chats.Add(chat);
		DbContext.Messages.Add(message);
		DbContext.SaveChanges();

		await _messageRepository.DeleteMessageAsync(message.Id);

		Assert.False(DbContext.Messages.Any(m => m.Id == message.Id));
	}

	[Fact]
	public async Task DeleteMessageAsync_MessageDoesntExist_ShouldThrowEntityNotFoundException()
	{
		var user = new User { Name = "admin" };
		var chat = new Chat { Name = "test", Admin = user };
		var message = new Message { Chat = chat, Sender = user, Content = "test", SentAt = DateTime.Now };

		await Assert.ThrowsAsync<EntityNotFoundException<Message>>(() => _messageRepository.DeleteMessageAsync(message.Id));
	}
}
