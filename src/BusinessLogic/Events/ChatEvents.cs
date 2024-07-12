using SimpleSignalrChat.BusinessLogic.DTOs;
using SimpleSignalrChat.BusinessLogic.Events.Interfaces;

namespace SimpleSignalrChat.BusinessLogic.Events;

public record ChatCreatedEvent (ChatInfoDto Chat) : IEvent;
public record ChatUpdatedEvent (ChatInfoDto Chat) : IEvent;
public record ChatDeletedEvent (ChatInfoDto Chat) : IEvent;