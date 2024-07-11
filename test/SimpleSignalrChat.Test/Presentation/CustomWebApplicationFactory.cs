using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleSignalrChat.DataAccess;

namespace SimpleSignalrChat.Test.Presentation;

class CustomWebApplicationFactory : WebApplicationFactory<Program>, IDisposable
{
	public ChatContext DbContext { get; }
	private const string InMemoryConnectionString = "DataSource=:memory:";
	private SqliteConnection _connection;

	public CustomWebApplicationFactory() : base()
	{
		_connection = new SqliteConnection(InMemoryConnectionString);
		_connection.Open();

		var options = new DbContextOptionsBuilder<ChatContext>()
				.UseSqlite(_connection)
				.Options;

		DbContext = new ChatContext(options);
		DbContext.Database.EnsureCreated();
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		base.ConfigureWebHost(builder);
		builder.ConfigureTestServices(services =>
		{
			services.AddSingleton(DbContext);
		});
	}

	public void Dispose()
	{
		_connection.Close();
		_connection.Dispose();
		base.Dispose();
	}
}
