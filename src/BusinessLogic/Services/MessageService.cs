using SimpleSignalrChat.BusinessLogic.Abstractions;
using SimpleSignalrChat.BusinessLogic.DTOs;
using SimpleSignalrChat.BusinessLogic.Events;
using SimpleSignalrChat.BusinessLogic.Events.Interfaces;
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
	private readonly IEventPublisher? _eventPublisher;

	public MessageService(
		IMessageRepository messageRepository,
		IChatRepository chatRepository,
		IUserRepository userRepository,
		IEventPublisher? eventPublisher = null)
	{
		_messageRepository = messageRepository;
		_chatRepository = chatRepository;
		_userRepository = userRepository;
		_eventPublisher = eventPublisher;
	}

	public async Task<Result<MessageInfoDto>> AddMessageAsync(int chatId, int userId, string content)
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
		MessageInfoDto messageDto = MessageInfoDto.From((await _messageRepository.AddMessageAsync(message))!);
		_eventPublisher?.Publish(new MessageCreatedEvent(messageDto));
		return messageDto;
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
		_eventPublisher?.Publish(new MessageDeletedEvent(id));
		return Result.Success;
	}

	public async Task<Result<List<MessageDto>>> GetChatMessagesAsync(int chatId)
	{
		Chat? chat = await _chatRepository.GetChatAsync(chatId);
		if (chat is null)
		{
			return new ChatNotFoundException(chatId);
		}

		return (await _messageRepository.GetChatMessagesAsync(chat)).Select(MessageDto.From).ToList();
	}

	public async Task<Result<MessageInfoDto>> GetMessageAsync(int id)
	{
		Message? message = await _messageRepository.GetMessageAsync(id);
		if (message is null)
		{
			return new MessageNotFoundException(id);
		}
		return MessageInfoDto.From(message);
	}
}
