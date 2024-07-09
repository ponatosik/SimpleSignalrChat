namespace SimpleSignalrChat.DataAccess.Entities;

public class Chat
{
	public int Id { get; set; }
	public string Name { get; set; }
	public User Admin { get; set; }
	public ICollection<Message> Messages { get; set; }
}
