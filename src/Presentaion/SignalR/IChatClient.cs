using SimpleSignalrChat.BusinessLogic.DTOs;

namespace SimpleSignalrChat.Presentaion.SignalR;

public interface IChatClient
{
	public Task ReceiveMessage(MessageDto message);
	public Task GetMessages(List<MessageDto> messages);
	public Task UserConnected(UserDto user);
	public Task DeleteMessage(int messageId);
	public Task ConnectToChat(ChatInfoDto chat);
	public Task ExitChat(string reason);
	public Task Error(string targetAction, string message);
}
