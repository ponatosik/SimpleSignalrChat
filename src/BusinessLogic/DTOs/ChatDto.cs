using SimpleSignalrChat.DataAccess.Entities;

namespace SimpleSignalrChat.BusinessLogic.DTOs;

public record ChatDto(int Id, string Name)
{
	public static ChatDto From(Chat chat)
	{
		return new ChatDto(chat.Id, chat.Name);
	}
}
