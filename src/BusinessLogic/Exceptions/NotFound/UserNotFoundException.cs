namespace SimpleSignalrChat.BusinessLogic.Exceptions.NotFound;

public class UserNotFoundException : NotFoundException
{
    public int UserId { get; }

    public UserNotFoundException(int entityKey) : base(GetMessage(entityKey))
    {
        UserId = entityKey;
    }

    private static string GetMessage(int entityKey) =>
        $"User with id {entityKey} not found.";
}
