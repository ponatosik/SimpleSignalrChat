namespace SimpleSignalrChat.BusinessLogic.Exceptions.NotFound;

public class ChatNotFoundException : NotFoundException
{
    public int ChatId { get; }

    public ChatNotFoundException(int entityKey) : base(GetMessage(entityKey))
    {
        ChatId = entityKey;
    }

    private static string GetMessage(int entityKey) =>
        $"Chat with id {entityKey} not found.";
}
