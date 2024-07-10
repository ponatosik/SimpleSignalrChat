using SimpleSignalrChat.BusinessLogic.Abstractions;
using SimpleSignalrChat.DataAccess.Entities;

namespace SimpleSignalrChat.BusinessLogic.Services.Interfaces;

public interface IMessageService
{
	public Task<Result<Message>> GetMessageAsync(int id);
	public Task<Result<List<Message>>> GetChatMessagesAsync(int chatId);
	public Task<Result<Message>> AddMessageAsync(int chatId, int userId, string content);
	public Task<Result> DeleteMessageAsync(int id, int userId);
}
