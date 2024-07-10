using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SimpleSignalrChat.DataAccess;

namespace SimpleSignalrChat.Test.DataAccess.Repositories;

public abstract class RepositoryTest : IDisposable
{
	private const string InMemoryConnectionString = "DataSource=:memory:";
	private readonly SqliteConnection _connection;

	protected readonly ChatContext DbContext;

	protected RepositoryTest()
	{
		_connection = new SqliteConnection(InMemoryConnectionString);
		_connection.Open();

		var options = new DbContextOptionsBuilder<ChatContext>()
				.UseSqlite(_connection)
				.Options;

		DbContext = new ChatContext(options);
		DbContext.Database.EnsureCreated();
	}

	public void Dispose()
	{
		_connection.Close();
	}
}
