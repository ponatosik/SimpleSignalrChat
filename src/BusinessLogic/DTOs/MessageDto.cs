using SimpleSignalrChat.DataAccess.Entities;

namespace SimpleSignalrChat.BusinessLogic.DTOs;

public record MessageDto(int Id, string Contet, int SenderId, DateTime SentAt)
{
	public static MessageDto From(Message message)
	{
		return new MessageDto(message.Id, message.Content, message.Sender.Id, message.SentAt);
	}

	public static MessageDto From(MessageInfoDto message)
	{
		return new MessageDto(message.Id, message.Contet, message.SenderId, message.SentAt);
	}
}
