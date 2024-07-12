using System.Reflection;
using SimpleSignalrChat.BusinessLogic.Events.Interfaces;

namespace SimpleSignalrChat.BusinessLogic.Events.Implementation;

public class EventPublisher : IEventPublisher
{
	private readonly Dictionary<Type, List<object?>> _handlers;

	public EventPublisher(IServiceProvider serviceProvider)
	{
		_handlers = [];

		var eventTypes = Assembly.GetExecutingAssembly()
			.GetTypes()
			.Where(tupe => typeof(IEvent).IsAssignableFrom(tupe) && !tupe.IsInterface && !tupe.IsAbstract)
			.ToList();

		foreach (var eventType in eventTypes)
		{
			var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
			var handlers = serviceProvider.GetServices(handlerType).ToList();

			if (handlers.Any())
			{
				_handlers[eventType] = handlers;
			}
		}
	}

	public async Task Publish<T>(T @event) where T : IEvent
	{
		if (_handlers.TryGetValue(typeof(T), out var handlersObj))
		{
			var handlers = handlersObj.Select(handler => handler as IEventHandler<T>);
			var tasks = handlers.Select(handler => handler?.Handle(@event) ?? Task.CompletedTask);
			await Task.WhenAll(tasks);
		}
	}
}