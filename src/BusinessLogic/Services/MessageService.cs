using SimpleSignalrChat.BusinessLogic.Abstractions;
using SimpleSignalrChat.BusinessLogic.Exceptions.NotEnoughPrivilege;
using SimpleSignalrChat.BusinessLogic.Exceptions.NotFound;
using SimpleSignalrChat.BusinessLogic.Services.Interfaces;
using SimpleSignalrChat.DataAccess.Entities;
using SimpleSignalrChat.DataAccess.Repositories.Interfaces;

namespace SimpleSignalrChat.BusinessLogic.Services;

public class MessageService : IMessageService
{
	private readonly IMessageRepository _messageRepository;
	private readonly IChatRepository _chatRepository;
	private readonly IUserRepository _userRepository;

	public MessageService(IMessageRepository messageRepository, IChatRepository chatRepository, IUserRepository userRepository)
	{
		_messageRepository = messageRepository;
		_chatRepository = chatRepository;
		_userRepository = userRepository;
	}

	public async Task<Result<Message>> AddMessageAsync(int chatId, int userId, string content)
	{
		User? user = await _userRepository.GetUserAsync(userId);
		if (user is null)
		{
			return new UserNotFoundException(userId);
		}

		Chat? chat = await _chatRepository.GetChatAsync(chatId);
		if (chat is null)
		{
			return new ChatNotFoundException(chatId);
		}

		Message message = new Message() { Chat = chat, Sender = user, Content = content, SentAt = DateTime.Now };
		return (await _messageRepository.AddMessageAsync(message))!;
	}

	public async Task<Result> DeleteMessageAsync(int id, int userId)
	{
		User? user = await _userRepository.GetUserAsync(userId);
		if (user is null)
		{
			return new UserNotFoundException(userId);
		}

		Message? message = await _messageRepository.GetMessageAsync(id);
		if (message is null)
		{
			return new MessageNotFoundException(id);
		}
		if (message.Sender.Id != user.Id)
		{
			return new NotEnoughPrivilegeException("Sender", "Delete message");
		}

		await _messageRepository.DeleteMessageAsync(id);
		return Result.Success;
	}

	public async Task<Result<List<Message>>> GetChatMessagesAsync(int chatId)
	{
		Chat? chat = await _chatRepository.GetChatAsync(chatId);
		if (chat is null)
		{
			return new ChatNotFoundException(chatId);
		}

		return await _messageRepository.GetChatMessagesAsync(chat);
	}

	public async Task<Result<Message>> GetMessageAsync(int id)
	{
		Message? message = await _messageRepository.GetMessageAsync(id);
		if (message is null)
		{
			return new MessageNotFoundException(id);
		}
		return message;
	}
}
