using Microsoft.EntityFrameworkCore;
using SimpleSignalrChat.DataAccess.Entities;
using SimpleSignalrChat.DataAccess.Exceptions;
using SimpleSignalrChat.DataAccess.Repositories.Interfaces;

namespace SimpleSignalrChat.DataAccess.Repositories;

public class MessageRepository : IMessageRepository
{
	private readonly ChatContext _chatContext;

	public MessageRepository(ChatContext chatContext)
	{
		_chatContext = chatContext;
	}

	public async Task<Message> AddMessageAsync(Message message)
	{
		User? sender = await _chatContext.Users.FindAsync(message.Sender.Id);
		if (sender is null)
		{
			throw new EntityNotFoundException<User>(message.Sender.Id);
		}

		Chat? chat = await _chatContext.Chats.FindAsync(message.Chat.Id);
		if(chat is null)
		{
			throw new EntityNotFoundException<Chat>(message.Chat.Id);
		}

		message.Chat = chat;
		message.Sender = sender;
		_chatContext.Messages.Add(message);
		_chatContext.SaveChanges();
		return message;
	}

	public Task DeleteMessageAsync(int id)
	{
		Message? message = _chatContext.Messages.Find(id);
		if (message is null)
		{
			return Task.FromException(new EntityNotFoundException<Message>(id));
		}

		_chatContext.Messages.Remove(message);
		return _chatContext.SaveChangesAsync();
	}

	public async Task<List<Message>> GetChatMessagesAsync(Chat chat)
	{
		Chat? chatEnity = await _chatContext.Chats
			.Include(chat => chat.Messages)
			.FirstOrDefaultAsync(chat => chat.Id == chat.Id);

		if(chatEnity is null)
		{
			throw new EntityNotFoundException<Chat>(chat.Id);
		}
			
		var chatMessages = chatEnity.Messages.ToList();
		_chatContext.Entry(chatEnity).State = EntityState.Detached;
		return chatMessages;
	}

	public Task<Message?> GetMessageAsync(int id)
	{
		return _chatContext.Messages
			.AsNoTracking()
			.Include(message => message.Chat)
			.Include(message => message.Sender)
			.FirstOrDefaultAsync(message => message.Id == id);
	}
}
