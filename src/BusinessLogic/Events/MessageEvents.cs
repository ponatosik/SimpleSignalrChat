using SimpleSignalrChat.BusinessLogic.DTOs;
using SimpleSignalrChat.BusinessLogic.Events.Interfaces;

namespace SimpleSignalrChat.BusinessLogic.Events;

public record MessageCreatedEvent (MessageInfoDto Message) : IEvent;
public record MessageDeletedEvent (int Id) : IEvent;
