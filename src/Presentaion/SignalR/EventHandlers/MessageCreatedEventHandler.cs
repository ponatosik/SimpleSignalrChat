using Microsoft.AspNetCore.SignalR;
using SimpleSignalrChat.BusinessLogic.DTOs;
using SimpleSignalrChat.BusinessLogic.Events;
using SimpleSignalrChat.BusinessLogic.Events.Interfaces;

namespace SimpleSignalrChat.Presentaion.SignalR.EventHandlers;

public class MessageCreatedEventHandler : IEventHandler<MessageCreatedEvent>
{
    private readonly IHubContext<ChatHub, IChatClient> _context;

    public MessageCreatedEventHandler(IHubContext<ChatHub, IChatClient> context)
    {
        _context = context;
    }

    public Task Handle(MessageCreatedEvent @event)
    {
        return _context.Clients
            .Group(@event.Message.ChatId.ToString())
            .ReceiveMessage(MessageDto.From(@event.Message));
    }
}
