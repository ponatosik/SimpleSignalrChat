using Microsoft.EntityFrameworkCore;
using SimpleSignalrChat.DataAccess.Entities;
using SimpleSignalrChat.DataAccess.Exceptions;
using SimpleSignalrChat.DataAccess.Repositories.Interfaces;

namespace SimpleSignalrChat.DataAccess.Repositories;

public class ChatRepository : IChatRepository
{
	private readonly ChatContext _chatContext;

	public ChatRepository(ChatContext chatContext)
	{
		_chatContext = chatContext;
	}

	public async Task<Chat> AddChatAsync(Chat chat)
	{
		User? admin = _chatContext.Users.Find(chat.Admin.Id);
		if (admin is null)
		{
			throw new EntityNotFoundException<User>(chat.Admin.Id);
		}
		chat.Admin = admin;
		_chatContext.Chats.Add(chat);
		await _chatContext.SaveChangesAsync();
		return chat;
	}

	public Task DeleteChatAsync(int id)
	{
		Chat? chat = _chatContext.Chats.Find(id);
		if (chat is null)
		{
			return Task.FromException(new EntityNotFoundException<Chat>(id));
		}
		_chatContext.Chats.Remove(chat);
		return _chatContext.SaveChangesAsync();
	}

	public Task<List<Chat>> GetAllChatsAsync()
	{
		return _chatContext.Chats
			.AsNoTracking()
			.ToListAsync();
	}

	public Task<Chat?> GetChatAsync(int id)
	{
		return _chatContext.Chats
			.Include(chat => chat.Admin)
			.AsNoTracking()
			.FirstOrDefaultAsync(chat => chat.Id == id);
	}

	public Task<List<Chat>> SearchChatsAsync(string? name, int? adminId)
	{
		return _chatContext.Chats
			.AsNoTracking()
			.Where(chat => string.IsNullOrEmpty(name) || chat.Name.Contains(name))
			.Where(chat => (adminId == null) || chat.Admin.Id == adminId)
			.ToListAsync();
	}

	public async Task<Chat?> UpdateChatAsync(int id, Chat newChat)
	{
		Chat? chat = await _chatContext.Chats
			.Include(chat => chat.Admin)
			.FirstOrDefaultAsync(chat => chat.Id == id);
		if (chat is null)
		{
			return null;
		}

		if (newChat.Admin is null)
		{
			newChat.Admin = chat.Admin;
		}

		_chatContext.Entry(chat).CurrentValues.SetValues(newChat);
		await _chatContext.SaveChangesAsync();
		return chat;
	}
}
