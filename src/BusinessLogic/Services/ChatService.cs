using SimpleSignalrChat.BusinessLogic.Abstractions;
using SimpleSignalrChat.BusinessLogic.DTOs;
using SimpleSignalrChat.BusinessLogic.Events;
using SimpleSignalrChat.BusinessLogic.Events.Interfaces;
using SimpleSignalrChat.BusinessLogic.Exceptions.NotEnoughPrivilege;
using SimpleSignalrChat.BusinessLogic.Exceptions.NotFound;
using SimpleSignalrChat.BusinessLogic.Services.Interfaces;
using SimpleSignalrChat.DataAccess.Entities;
using SimpleSignalrChat.DataAccess.Exceptions;
using SimpleSignalrChat.DataAccess.Repositories.Interfaces;

namespace SimpleSignalrChat.BusinessLogic.Services;

public class ChatService : IChatService
{
	private readonly IChatRepository _chatRepository;
	private readonly IUserRepository _userRepository;
	private readonly IEventPublisher? _eventPublisher;

	public ChatService(
		IChatRepository chatRepository,
		IUserRepository userRepository,
		IEventPublisher? eventPublisher = null)
	{
		_chatRepository = chatRepository;
		_userRepository = userRepository;
		_eventPublisher = eventPublisher;
	}

	public async Task<Result<ChatInfoDto>> CreateChatAsync(int adminId, string name)
	{
		Chat chat = new Chat() { Admin = new User() { Id = adminId }, Name = name };
		try
		{
			var chatDto = ChatInfoDto.From(await _chatRepository.AddChatAsync(chat));
			_eventPublisher?.Publish(new ChatCreatedEvent(chatDto));
			return chatDto;
		}
		catch (EntityNotFoundException<User> exception)
		{
			return new UserNotFoundException(exception.EntityKey);
		}
	}

	public async Task<Result> DeleteChatAsync(int id, int userId)
	{
		User? user = await _userRepository.GetUserAsync(userId);
		if (user is null)
		{
			return new UserNotFoundException(userId);
		}

		Chat? chat = await _chatRepository.GetChatAsync(id);
		if(chat is null)
		{
			return new ChatNotFoundException(id);
		}

		if (chat.Admin.Id != user.Id)
		{
			return new NotEnoughPrivilegeException("Admin", $"Delete chat \"{chat.Name}\"");
		}

		await _chatRepository.DeleteChatAsync(id);
		_eventPublisher?.Publish(new ChatDeletedEvent(id));
		return Result.Success;
	}

	public async Task<Result<List<ChatDto>>> GetAllChatsAsync(string? nameFilter = null, int? adminId = null)
	{
		if(nameFilter is null && adminId is null) {
			return (await _chatRepository.GetAllChatsAsync()).Select(ChatDto.From).ToList();
		}
		else
		{
			return (await _chatRepository.SearchChatsAsync(nameFilter, adminId)).Select(ChatDto.From).ToList();
		}
	}

	public async Task<Result<ChatInfoDto>> GetChatAsync(int id)
	{
		Chat? chat = await _chatRepository.GetChatAsync(id);
		if(chat is null)
		{
			return new ChatNotFoundException(id);
		}
		return ChatInfoDto.From(chat!);
	}

	public async Task<Result<ChatInfoDto>> UpdateChatAsync(int id, string newName, int userId)
	{
		User? user = await _userRepository.GetUserAsync(userId);
		if (user is null)
		{
			return new UserNotFoundException(userId);
		}

		Chat? chat = await _chatRepository.GetChatAsync(id);
		if(chat is null)
		{
			return new ChatNotFoundException(id);
		}

		if (chat.Admin.Id != user.Id)
		{
			return new NotEnoughPrivilegeException("Admin", $"Change chat \"{chat.Name}\"");
		}

		Chat newChat = new Chat() { Id = id, Admin = new User() , Name = newName };
		ChatInfoDto chatDto = ChatInfoDto.From((await _chatRepository.UpdateChatAsync(id, newChat))!);
		_eventPublisher?.Publish(new ChatUpdatedEvent(chatDto));
		return chatDto;
	}
}
