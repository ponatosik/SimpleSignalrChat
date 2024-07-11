using SimpleSignalrChat.DataAccess.Entities;

namespace SimpleSignalrChat.BusinessLogic.DTOs;

public record UserDto(int Id, string Name)
{
	public static UserDto From(User user)
	{
		return new UserDto(user.Id, user.Name);
	}
}
