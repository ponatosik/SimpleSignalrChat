namespace SimpleSignalrChat.DataAccess.Exceptions;

public class EntityNotFoundException<T> : Exception
{
	public int EntityKey { get; }

	public EntityNotFoundException(int entityKey) 
		: base(GetMessage(entityKey)) 
	{
		EntityKey = entityKey;
	}

	public EntityNotFoundException(int entityKey, Exception innerException)
		: base(GetMessage(entityKey), innerException) 
	{
		EntityKey = entityKey;
	}

	private static string GetMessage(int entityKey) =>
		$"Entity of type {typeof(T)} with id {entityKey} was not found.";
}