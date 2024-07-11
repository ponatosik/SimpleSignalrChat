using SimpleSignalrChat.DataAccess.Entities;

namespace SimpleSignalrChat.BusinessLogic.DTOs;

public record ChatInfoDto(int Id, string Name, int AdminId, string AdminName)
{
	public static ChatInfoDto From(Chat chat)
	{
		return new ChatInfoDto(chat.Id, chat.Name, chat.Admin.Id, chat.Admin.Name);
	}
}
