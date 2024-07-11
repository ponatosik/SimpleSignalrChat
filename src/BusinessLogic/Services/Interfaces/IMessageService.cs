using SimpleSignalrChat.BusinessLogic.Abstractions;
using SimpleSignalrChat.BusinessLogic.DTOs;

namespace SimpleSignalrChat.BusinessLogic.Services.Interfaces;

public interface IMessageService
{
	public Task<Result<MessageInfoDto>> GetMessageAsync(int id);
	public Task<Result<List<MessageDto>>> GetChatMessagesAsync(int chatId);
	public Task<Result<MessageInfoDto>> AddMessageAsync(int chatId, int userId, string content);
	public Task<Result> DeleteMessageAsync(int id, int userId);
}
