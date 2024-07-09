using Microsoft.EntityFrameworkCore;
using SimpleSignalrChat.DataAccess.Entities;

namespace SimpleSignalrChat.DataAccess;

public class ChatContext : DbContext
{
	public DbSet<Chat> Chats { get; set; }
	public DbSet<User> Users { get; set; }
	public DbSet<Message> Messages { get; set; }
}
