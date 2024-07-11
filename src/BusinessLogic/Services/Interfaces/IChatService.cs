using SimpleSignalrChat.BusinessLogic.Abstractions;
using SimpleSignalrChat.BusinessLogic.DTOs;

namespace SimpleSignalrChat.BusinessLogic.Services.Interfaces;

public interface IChatService
{
	public Task<Result<ChatInfoDto>> GetChatAsync(int id);
	public Task<Result<List<ChatDto>>> GetAllChatsAsync(string? nameFilter = null, int? adminId = null);
	public Task<Result<ChatInfoDto>> CreateChatAsync(int adminId, string name);
	public Task<Result<ChatInfoDto>> UpdateChatAsync(int id, string newName, int userId);
	public Task<Result> DeleteChatAsync(int id, int userId);
}
