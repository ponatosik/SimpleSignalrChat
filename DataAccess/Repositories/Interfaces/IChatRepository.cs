using SimpleSignalrChat.DataAccess.Entities;

namespace SimpleSignalrChat.DataAccess.Repositories.Interfaces;

public interface IChatRepository
{
	public Task<Chat?> GetChatAsync(int id);
	public Task<List<Chat>> GetAllChatsAsync();
	public Task<List<Chat>> SearchChatsAsync(string? name, int? adminId);
	public Task<Chat> AddChatAsync(Chat chat);
	public Task<Chat?> UpdateChatAsync(int id, Chat newChat);
	public Task DeleteChatAsync(int id);
}
