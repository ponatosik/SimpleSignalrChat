using SimpleSignalrChat.DataAccess.Entities;
using SimpleSignalrChat.DataAccess.Exceptions;
using SimpleSignalrChat.DataAccess.Repositories;
using SimpleSignalrChat.DataAccess.Repositories.Interfaces;

namespace SimpleSignalrChat.Test.DataAccess.Repositories;

public class ChatRepositoryTest : RepositoryTest
{
	private readonly IChatRepository _chatRepository;

	public ChatRepositoryTest() : base() 
	{
		_chatRepository = new ChatRepository(DbContext);
	}


	[Fact]
	public async Task GetChatAsync_UserExists_ShouldReturnChat()
	{
		var user = new User { Name = "admin" };
		var chat = new Chat { Name = "test", Admin = user };
		DbContext.Users.Add(user);
		DbContext.Chats.Add(chat);
		DbContext.SaveChanges();

		var result = await _chatRepository.GetChatAsync(1);

		Assert.NotNull(result);
		Assert.Equal(chat.Name, result.Name);
		Assert.Equal(chat.Admin.Id, result.Admin.Id);
		Assert.Equal(chat.Admin.Name, result.Admin.Name);
	}

	[Fact]
	public async Task GetChatAsync_UserDoesntExist_ShouldReturnNull()
	{
		var user = new User { Name = "admin" };
		var chat = new Chat { Name = "test", Admin = user };
		DbContext.Users.Add(user);
		DbContext.Chats.Add(chat);
		DbContext.SaveChanges();

		var result = await _chatRepository.GetChatAsync(-999);
		
		Assert.Null(result);
	}

	[Fact]
	public async Task GetAllChatsAsync_Allways_ShouldRerurnChats()
	{
		var user1 = new User { Name = "admin1" };
		var user2 = new User { Name = "admin2" };
		var chat1 = new Chat { Name = "test1", Admin = user1 };
		var chat2 = new Chat { Name = "test2", Admin = user1 };
		var chat3 = new Chat { Name = "test3", Admin = user2 };
		DbContext.Users.AddRange(user1, user2);
		DbContext.Chats.AddRange(chat1, chat2, chat3);
		DbContext.SaveChanges();

		var result = await _chatRepository.GetAllChatsAsync();

		Assert.NotEmpty(result);
		Assert.Equal(3, result.Count());
		Assert.Contains(result, c => c.Name == chat1.Name);
		Assert.Contains(result, c => c.Name == chat2.Name);
		Assert.Contains(result, c => c.Name == chat3.Name);
	}

	[Fact]
	public async Task SearchChatsAsync_SearchByName_ShouldReturnChatsContainingSearchStirng()
	{
		var user1 = new User { Name = "admin1" };
		var user2 = new User { Name = "admin2" };
		var chat1 = new Chat { Name = "test aa bb", Admin = user1 };
		var chat2 = new Chat { Name = "test bb cc", Admin = user1 };
		var chat3 = new Chat { Name = "test aa cc", Admin = user2 };
		DbContext.Users.AddRange(user1, user2);
		DbContext.Chats.AddRange(chat1, chat2, chat3);
		DbContext.SaveChanges();

		var result = await _chatRepository.SearchChatsAsync("aa", null);

		Assert.NotEmpty(result);
		Assert.Equal(2, result.Count());
		Assert.Contains(result, c => c.Name == chat1.Name);
		Assert.Contains(result, c => c.Name == chat3.Name);
	}

	[Fact]
	public async Task SearchChatsAsync_SearchByAdmin_ShouldReturnChatsCreatedByUser()
	{
		var user1 = new User { Name = "admin1" };
		var user2 = new User { Name = "admin2" };
		var chat1 = new Chat { Name = "test aa bb", Admin = user1 };
		var chat2 = new Chat { Name = "test bb cc", Admin = user1 };
		var chat3 = new Chat { Name = "test aa cc", Admin = user2 };
		DbContext.Users.AddRange(user1, user2);
		DbContext.Chats.AddRange(chat1, chat2, chat3);
		DbContext.SaveChanges();

		var result = await _chatRepository.SearchChatsAsync(null, user1.Id);

		Assert.NotEmpty(result);
		Assert.Equal(2, result.Count());
		Assert.Contains(result, c => c.Name == chat1.Name);
		Assert.Contains(result, c => c.Name == chat2.Name);
	}

	[Fact]
	public async Task AddChatAsync_ExistingAdmin_ShouldReturnChat()
	{
		var user = new User { Name = "admin" };
		DbContext.Users.Add(user);
		DbContext.SaveChanges();

		var chat = new Chat { Name = "test", Admin = user };
		var result = await _chatRepository.AddChatAsync(chat);

		Assert.NotNull(result);
		Assert.Equal(chat.Name, result.Name);
		Assert.Equal(chat.Admin.Id, result.Admin.Id);
		Assert.Equal(chat.Admin.Name, result.Admin.Name);
	}

	[Fact]
	public async Task AddChatAsync_NonExistingAdmin_ShouldThrowEntityNotFound()
	{
		var chat = new Chat { Name = "test", Admin = new User { Name = "Not registered" } };

		await Assert.ThrowsAsync<EntityNotFoundException<User>>(() => _chatRepository.AddChatAsync(chat));
	}

	[Fact]
	public async Task AddChatAsync_ValidChat_ShouldPersistChat()
	{
		var user = new User { Name = "admin" };
		DbContext.Users.Add(user);
		DbContext.SaveChanges();

		var chat = new Chat { Name = "test", Admin = user };
		var result = await _chatRepository.AddChatAsync(chat);
		
		Assert.True(DbContext.Chats.Any(chat => 
			chat.Id == result.Id &&
			chat.Name == result.Name &&
			chat.Admin.Id == result.Admin.Id &&
			chat.Admin.Name == result.Admin.Name));
	}

	[Fact]
	public async Task UpdateChatAsync_ChatExists_ShouldUpdateChat()
	{
		var user1 = new User { Name = "admin1" };
		var user2 = new User { Name = "admin2" };
		var chat = new Chat { Name = "test2", Admin = user1 };
		DbContext.Users.AddRange(user1, user2);
		DbContext.Chats.Add(chat);
		DbContext.SaveChanges();

		var newChat = new Chat { Name = "test2", Admin = chat.Admin };
		await _chatRepository.UpdateChatAsync(chat.Id, newChat);

		var updatedChat = DbContext.Chats.Find(chat.Id);
		Assert.Equal(newChat.Name, updatedChat.Name);
		Assert.Equal(newChat.Admin.Id, updatedChat.Admin.Id);
	}

	[Fact]
	public async Task UpdateChatAsync_ChatDoesntExist_ShouldReturnNull()
	{
		var newChat = new Chat { Name = "not in database", Admin = new User { Name = "not in database" } };
		var result = await _chatRepository.UpdateChatAsync(-999, newChat);

		Assert.Null(result);
	}

	[Fact]
	public async Task DeleteChatAsync_ChatExists_ShouldDeleteChat()
	{
		var user = new User { Name = "admin" };
		var chat = new Chat { Name = "test", Admin = user };
		DbContext.Users.Add(user);
		DbContext.Chats.Add(chat);
		DbContext.SaveChanges();

		await _chatRepository.DeleteChatAsync(chat.Id);

		Assert.False(DbContext.Chats.Any(c => c.Id == chat.Id && c.Name == chat.Name));
	}

	[Fact]
	public async Task DeleteChatAsync_ChatDoesntExist_ShouldThrowEntityNotFound()
	{
		var user = new User { Name = "admin" };
		var chat = new Chat { Name = "test", Admin = user };
		DbContext.Users.Add(user);
		DbContext.Chats.Add(chat);
		DbContext.SaveChanges();
		
		await Assert.ThrowsAsync<EntityNotFoundException<Chat>>(() => _chatRepository.DeleteChatAsync(-999));
	}
}
