using Microsoft.AspNetCore.SignalR;

namespace SimpleSignalrChat.Presentaion.SignalR.Services;

public interface IHubGroupsManager<THub> where THub : Hub
{
	Task AddToGroupAsync(string? groupName, string connectionId);
	Task RemoveFromGroupAsync(string groupName, string connectionId);
	Task RemoveAllFromGroupAsync(string groupName);
	Task<IEnumerable<string>> GetConnectionsInGroupAsync(string groupName);
	Task<string?> GetUserGroupAsync(string connectionId);

}
