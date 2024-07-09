using SimpleSignalrChat.DataAccess.Entities;

namespace SimpleSignalrChat.DataAccess.Repositories.Interfaces;

public interface IMessageRepository
{
	public Task<Message?> GetMessageAsync(int id);
	public Task<List<Message>> GetChatMessagesAsync(Chat chat);
	public Task<Message> AddMessareAsync(Message message);
	public Task DeleteMessageAsync(int id);

}
