using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpleSignalrChat.BusinessLogic.DTOs;
using SimpleSignalrChat.DataAccess.Entities;
using SimpleSignalrChat.Presentaion.Rest.Requests;
using System.Net;
using System.Net.Http.Json;

namespace SimpleSignalrChat.Test.Presentation.Rest;

public class ChatsControllerTest : ApiControllerTest
{
	private User[] _registeredUsers =
	{
		new User { Name = "test user 1" },
		new User { Name = "test user 2" },
		new User { Name = "test user 3" }
	};

	public ChatsControllerTest() : base()
	{
		DbContext.Users.AddRange(_registeredUsers);
		DbContext.SaveChanges();
	}

	[Fact]
	public async Task Post_ValidRequest_ShouldReturnCreatedChat()
	{
		var request = new CreateChatRequest("test chat");
		var admin = _registeredUsers[0];

		Client.DefaultRequestHeaders.Add("Authorization", admin.Id.ToString());
		var response = await Client.PostAsJsonAsync("api/chats", request);
		response.EnsureSuccessStatusCode();

		var data = JsonConvert.DeserializeObject<ChatDto>(await response.Content.ReadAsStringAsync());
		Assert.NotNull(data);
		Assert.Equal(request.ChatName, data.Name);
	}

	[Fact]
	public async Task Get_ChatExists_ShouldReturnChatInfo()
	{
		var admin = _registeredUsers[0];
		var chat = new Chat { Name = "test chat", Admin = admin };
		DbContext.Chats.Add(chat);
		DbContext.SaveChanges();

		var response = await Client.GetAsync($"api/chats/{chat.Id}");
		response.EnsureSuccessStatusCode();

		var data = JsonConvert.DeserializeObject<ChatDto>(await response.Content.ReadAsStringAsync());
		Assert.NotNull(data);
		Assert.Equal(chat.Name, data.Name);
	}

	[Fact]
	public async Task Get_AllChats_ShouldReturnAllChats()
	{
		var chats = new List<Chat>
		{
			new Chat { Name = "test chat 1 marked" , Admin = _registeredUsers[0]},
			new Chat { Name = "test chat 2" , Admin = _registeredUsers[0]},
			new Chat { Name = "test chat 3" , Admin = _registeredUsers[1]},
			new Chat { Name = "test chat 4 marked" , Admin = _registeredUsers[2]}
		};
		DbContext.Chats.AddRange(chats);
		DbContext.SaveChanges();

		var response = await Client.GetAsync("api/chats");
		response.EnsureSuccessStatusCode();

		var data = JsonConvert.DeserializeObject<List<ChatDto>>(await response.Content.ReadAsStringAsync());

		Assert.NotNull(data);
		Assert.Collection(data,
			item1 => Assert.Equal("test chat 1 marked", item1.Name),
			item2 => Assert.Equal("test chat 2", item2.Name),
			item3 => Assert.Equal("test chat 3", item3.Name),
			item4 => Assert.Equal("test chat 4 marked", item4.Name));
	}

	[Fact]
	public async Task Get_AllChatsWithNameFilter_ShouldReturnFilteredChats()
	{
		var chats = new List<Chat>
		{
			new Chat { Name = "test chat 1 marked" , Admin = _registeredUsers[0]},
			new Chat { Name = "test chat 2" , Admin = _registeredUsers[0]},
			new Chat { Name = "test chat 3" , Admin = _registeredUsers[1]},
			new Chat { Name = "test chat 4 marked" , Admin = _registeredUsers[2]}
		};
		DbContext.Chats.AddRange(chats);
		DbContext.SaveChanges();

		var response = await Client.GetAsync("api/chats/?filter=marked");
		response.EnsureSuccessStatusCode();

		var data = JsonConvert.DeserializeObject<List<ChatDto>>(await response.Content.ReadAsStringAsync());
		Assert.NotNull(data);
		Assert.Collection(data,
			item1 => Assert.Equal("test chat 1 marked", item1.Name),
			item2 => Assert.Equal("test chat 4 marked", item2.Name));
	}

	[Fact]
	public async Task Get_AllChatsWithAdminFilter_ShouldReturnFilteredChats()
	{
		var admin = _registeredUsers[0];
		var chats = new List<Chat>
		{
			new Chat { Name = "test chat 1 marked" , Admin = admin},
			new Chat { Name = "test chat 2" , Admin = admin},
			new Chat { Name = "test chat 3" , Admin = _registeredUsers[1]},
			new Chat { Name = "test chat 4 marked" , Admin = _registeredUsers[2]}
		};
		DbContext.Chats.AddRange(chats);
		DbContext.SaveChanges();

		var response = await Client.GetAsync($"api/chats/?adminId={admin.Id}");
		response.EnsureSuccessStatusCode();

		var data = JsonConvert.DeserializeObject<List<ChatDto>>(await response.Content.ReadAsStringAsync());
		Assert.NotNull(data);
		Assert.Collection(data,
			item1 => Assert.Equal("test chat 1 marked", item1.Name),
			item2 => Assert.Equal("test chat 2", item2.Name));
	}

	[Fact]
	public async Task Get_AllChatsWithChatNameAndAdminFilter_ShouldReturnFilteredChats()
	{
		var admin = _registeredUsers[0];
		var chats = new List<Chat>
		{
			new Chat { Name = "test chat 1 marked" , Admin = admin},
			new Chat { Name = "test chat 2" , Admin = admin},
			new Chat { Name = "test chat 3" , Admin = _registeredUsers[1]},
			new Chat { Name = "test chat 4 marked" , Admin = _registeredUsers[2]}
		};
		DbContext.Chats.AddRange(chats);
		DbContext.SaveChanges();

		var response = await Client.GetAsync($"api/chats/?filter=marked&adminId={admin.Id}");
		response.EnsureSuccessStatusCode();

		var data = JsonConvert.DeserializeObject<List<ChatDto>>(await response.Content.ReadAsStringAsync());
		Assert.NotNull(data);
		Assert.Collection(data,
			item1 => Assert.Equal("test chat 1 marked", item1.Name));
	}

	[Fact]
	public async Task Get_ChatDoesntExist_ShouldReturn404NotFound()
	{
		var nonexistentChatId = -999;

		var response = await Client.GetAsync($"api/chats/{nonexistentChatId}");
		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

		var details = JsonConvert.DeserializeObject<ProblemDetails>(await response.Content.ReadAsStringAsync());

		Assert.NotNull(details);
		Assert.Equal(404, details.Status);
	}

	[Fact]
	public async Task Delete_ChatExistsAndUserIsAdmin_ShouldReturnSuccess()
	{
		var admin = _registeredUsers[0];
		var chat = new Chat { Name = "test chat", Admin = admin };
		DbContext.Chats.Add(chat);
		DbContext.SaveChanges();

		Client.DefaultRequestHeaders.Add("Authorization", admin.Id.ToString());
		var response = await Client.DeleteAsync($"api/chats/{chat.Id}");
		response.EnsureSuccessStatusCode();
	}

	[Fact]
	public async Task Delete_ChatExistsAndUserIsNotAdmin_ShouldReturn403Forbidden()
	{
		var admin = _registeredUsers[0];
		var reqularUser = _registeredUsers[1];
		var chat = new Chat { Name = "test chat", Admin = admin };
		DbContext.Chats.Add(chat);
		DbContext.SaveChanges();

		Client.DefaultRequestHeaders.Add("Authorization", reqularUser.Id.ToString());
		var response = await Client.DeleteAsync($"api/chats/{chat.Id}");
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

		var details = JsonConvert.DeserializeObject<ProblemDetails>(await response.Content.ReadAsStringAsync());
		Assert.NotNull(details);
		Assert.Equal(403, details.Status);
	}
}
