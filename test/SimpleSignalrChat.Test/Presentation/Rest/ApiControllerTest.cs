using SimpleSignalrChat.DataAccess;

namespace SimpleSignalrChat.Test.Presentation.Rest;

public class ApiControllerTest : IDisposable
{
	internal CustomWebApplicationFactory _factory;
	protected HttpClient Client { get; }
	protected ChatContext DbContext { get; }

	public ApiControllerTest()
	{
		_factory = new CustomWebApplicationFactory();
		Client = _factory.CreateClient();
		DbContext = _factory.DbContext;
	}

	public void Dispose()
	{
		DbContext.Database.EnsureDeleted();
		_factory.Dispose();
		Client.Dispose();
	}
}
