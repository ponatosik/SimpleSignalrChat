namespace SimpleSignalrChat.BusinessLogic.Exceptions.NotFound;

public class MessageNotFoundException : NotFoundException
{
    public int MessageId { get; }

    public MessageNotFoundException(int entityKey) : base(GetMessage(entityKey))
    {
        MessageId = entityKey;
    }

    private static string GetMessage(int entityKey) =>
        $"Message with id {entityKey} not found.";
}
