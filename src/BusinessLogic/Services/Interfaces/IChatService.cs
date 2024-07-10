using SimpleSignalrChat.BusinessLogic.Abstractions;
using SimpleSignalrChat.DataAccess.Entities;

namespace SimpleSignalrChat.BusinessLogic.Services.Interfaces;

public interface IChatService
{
	public Task<Result<Chat>> GetChatAsync(int id);
	public Task<Result<List<Chat>>> GetAllChatsAsync(string? nameFilter = null, int? adminId = null);
	public Task<Result<Chat>> CreateChatAsync(int adminId, string name);
	public Task<Result<Chat>> UpdateChatAsync(int id, string newName, int userId);
	public Task<Result> DeleteChatAsync(int id, int userId);
}
