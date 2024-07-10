namespace SimpleSignalrChat.DataAccess.Entities;

public class Message
{
	public int Id { get; set; }
	public string Content { get; set; }
	public DateTime SentAt { get; set; }
	public User Sender { get; set; }
	public Chat Chat { get; set; }
}
