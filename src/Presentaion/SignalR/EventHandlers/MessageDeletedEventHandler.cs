using Microsoft.AspNetCore.SignalR;
using SimpleSignalrChat.BusinessLogic.DTOs;
using SimpleSignalrChat.BusinessLogic.Events;
using SimpleSignalrChat.BusinessLogic.Events.Interfaces;

namespace SimpleSignalrChat.Presentaion.SignalR;

public class MessageDeletedEventHandler : IEventHandler<MessageDeletedEvent>
{
	private readonly IHubContext<ChatHub, IChatClient> _context;

	public MessageDeletedEventHandler(IHubContext<ChatHub, IChatClient> context)
	{
		_context = context;
	}

	public Task Handle(MessageDeletedEvent @event)
	{
		return _context.Clients
			.Group(@event.Message.ChatId.ToString())
			.ReceiveMessage(MessageDto.From(@event.Message));
	}
}
