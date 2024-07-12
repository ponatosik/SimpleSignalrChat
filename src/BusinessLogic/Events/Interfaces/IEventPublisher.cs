namespace SimpleSignalrChat.BusinessLogic.Events.Interfaces;

public interface IEventPublisher
{
	public Task Publish<T>(T @event) where T : IEvent;
}
