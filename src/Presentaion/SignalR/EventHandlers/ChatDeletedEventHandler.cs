using Microsoft.AspNetCore.SignalR;
using SimpleSignalrChat.BusinessLogic.DTOs;
using SimpleSignalrChat.BusinessLogic.Events;
using SimpleSignalrChat.BusinessLogic.Events.Interfaces;
using SimpleSignalrChat.Presentaion.SignalR.Services;

namespace SimpleSignalrChat.Presentaion.SignalR;

public class ChatDeletedEventHandler : IEventHandler<ChatDeletedEvent>
{
	private readonly IHubContext<ChatHub, IChatClient> _context;
	private readonly IHubGroupsManager<ChatHub> _groups;

	public ChatDeletedEventHandler(IHubContext<ChatHub, IChatClient> context, IHubGroupsManager<ChatHub> groups)
	{
		_context = context;
		_groups = groups;
	}

	public async Task Handle(ChatDeletedEvent @event)
	{
		await _context.Clients
			.Group(@event.Chat.Id.ToString())
			.ExitChat("Chat was deleted");
		
		await _groups.RemoveAllFromGroupAsync(@event.Chat.Id.ToString());
	}
}
