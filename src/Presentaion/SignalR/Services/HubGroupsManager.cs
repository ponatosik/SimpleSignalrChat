using Microsoft.AspNetCore.SignalR;

namespace SimpleSignalrChat.Presentaion.SignalR.Services;

public class HubGroupsManager<THub> : IHubGroupsManager<THub>
	where THub : Hub
{
	private readonly IHubContext<THub> _hubContext;
	private readonly Dictionary<string, HashSet<string>> _groupConnections;
	private readonly Dictionary<string, string?> _userGroupConnections;

	public HubGroupsManager(IHubContext<THub> hubContext)
	{
		_hubContext = hubContext;
		_groupConnections = new Dictionary<string, HashSet<string>>();
		_userGroupConnections = new Dictionary<string, string?>();
	}

	public async Task AddToGroupAsync(string? groupName, string connectionId)
	{
		if(string.IsNullOrEmpty(groupName))
		{
			return;
		}
		if (!_groupConnections.ContainsKey(groupName))
		{
			_groupConnections[groupName] = new HashSet<string>();
		}
		_groupConnections[groupName].Add(connectionId);
		_userGroupConnections[connectionId] = groupName;

		await _hubContext.Groups.AddToGroupAsync(connectionId, groupName);
	}

	public Task<IEnumerable<string>> GetConnectionsInGroupAsync(string groupName)
	{
		if (_groupConnections.TryGetValue(groupName, out var connectionSet))
		{
			return Task.FromResult(connectionSet.AsEnumerable());
		}
		return Task.FromResult(Enumerable.Empty<string>());
	}

	public Task<string?> GetUserGroupAsync(string connectionId)
	{
		if (_userGroupConnections.TryGetValue(connectionId, out var groupName))
		{
			return Task.FromResult<string?>(groupName);
		}
		return Task.FromResult<string?>(null);
	}

	public async Task RemoveAllFromGroupAsync(string groupName)
	{
		IEnumerable<string> connections;
		if (!_groupConnections.TryGetValue(groupName, out var connectionSet))
		{
			return;
		}
		connections = connectionSet.ToList();
		_groupConnections.Remove(groupName);

		foreach (var connectionId in connections)
		{
			_userGroupConnections.Remove(connectionId);
			await _hubContext.Groups.RemoveFromGroupAsync(connectionId, groupName);
		}
	}

	public async Task RemoveFromGroupAsync(string groupName, string connectionId)
	{
		if (_groupConnections.ContainsKey(groupName))
		{
			_groupConnections[groupName].Remove(connectionId);
			if (_groupConnections[groupName].Count == 0)
			{
				_groupConnections.Remove(groupName);
			}
		}

		_userGroupConnections.Remove(connectionId);
		await _hubContext.Groups.RemoveFromGroupAsync(connectionId, groupName);
	}
}
