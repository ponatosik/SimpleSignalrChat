using SimpleSignalrChat.BusinessLogic.Abstractions;
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

	public ChatService(IChatRepository chatRepository, IUserRepository userRepository)
	{
		_chatRepository = chatRepository;
		_userRepository = userRepository;
	}

	public async Task<Result<Chat>> CreateChatAsync(int adminId, string name)
	{
		Chat chat = new Chat() { Admin = new User() { Id = adminId }, Name = name };
		try
		{
			return await _chatRepository.AddChatAsync(chat);
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
		return Result.Success;
	}

	public async Task<Result<List<Chat>>> GetAllChatsAsync(string? nameFilter = null, int? adminId = null)
	{
		if(nameFilter is null && adminId is null) {
			return await _chatRepository.GetAllChatsAsync();
		}
		else
		{
			return await _chatRepository.SearchChatsAsync(nameFilter, adminId);
		}
	}

	public async Task<Result<Chat>> GetChatAsync(int id)
	{
		Chat? chat = await _chatRepository.GetChatAsync(id);
		if(chat is null)
		{
			return new ChatNotFoundException(id);
		}
		return chat!;
	}

	public async Task<Result<Chat>> UpdateChatAsync(int id, string newName, int userId)
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
		return (await _chatRepository.UpdateChatAsync(id, newChat))!;
	}
}
