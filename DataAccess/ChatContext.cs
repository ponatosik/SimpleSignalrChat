using Microsoft.EntityFrameworkCore;
using SimpleSignalrChat.DataAccess.Entities;

namespace SimpleSignalrChat.DataAccess;

public class ChatContext : DbContext
{
	private DbSet<Chat> Chats { get; set; }
	private DbSet<User> Users { get; set; }
	private DbSet<Message> Messages { get; set; }
}
