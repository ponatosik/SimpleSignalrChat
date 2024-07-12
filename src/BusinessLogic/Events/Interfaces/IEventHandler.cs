namespace SimpleSignalrChat.BusinessLogic.Events.Interfaces;

public interface IEventHandler<T> where T : IEvent
{
	public Task Handle(T @event);
}
