using SimpleSignalrChat.DataAccess.Entities;

namespace SimpleSignalrChat.BusinessLogic.DTOs;

public record MessageInfoDto(int Id, int ChatId, string Contet, int SenderId, string SenderName, DateTime SentAt)
{
	public static MessageInfoDto From(Message message)
	{
		return new MessageInfoDto(
			message.Id,
			message.Chat.Id,
			message.Content,
			message.Sender.Id,
			message.Sender.Name,
			message.SentAt);
	}
}
