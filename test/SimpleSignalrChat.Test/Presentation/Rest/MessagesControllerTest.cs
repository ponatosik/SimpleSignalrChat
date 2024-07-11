using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SimpleSignalrChat.BusinessLogic.DTOs;
using SimpleSignalrChat.DataAccess.Entities;
using SimpleSignalrChat.Presentaion.Rest.Requests;
using System.Net;
using System.Net.Http.Json;

namespace SimpleSignalrChat.Test.Presentation.Rest;

public class MessagesControllerTest : ApiControllerTest
{
	private User[] _registeredUsers;
	private Chat[] _registeredChats;

	public MessagesControllerTest() : base()
	{
		_registeredUsers = new[]
		{
			new User { Name = "test user 1" },
			new User { Name = "test user 2" },
			new User { Name = "test user 3" }
		};
		_registeredChats = new[]
		{
			new Chat { Name = "test chat 1", Admin = _registeredUsers[0] },
			new Chat { Name = "test chat 2", Admin = _registeredUsers[0] },
			new Chat { Name = "test chat 3", Admin = _registeredUsers[1] }
		};

		DbContext.Users.AddRange(_registeredUsers);
		DbContext.Chats.AddRange(_registeredChats);
		DbContext.SaveChanges();
	}

	[Fact]
	public async Task Get_MessageExits_ShouldReturnMessages()
	{
		var chat = _registeredChats[0];
		var sender = _registeredUsers[1];
		var message = new Message { Chat = chat, Sender = sender, Content = "test message" };
		DbContext.Messages.Add(message);
		DbContext.SaveChanges();

		var response = await Client.GetAsync($"api/chats/{chat.Id}/messages/{message.Id}");
		response.EnsureSuccessStatusCode();

		var data = JsonConvert.DeserializeObject<MessageDto>(await response.Content.ReadAsStringAsync());

		Assert.NotNull(data);
		Assert.Equal(message.Content, data.Contet);
	}

	[Fact]
	public async Task Get_MessageDoesntExits_ShouldReturnNotFound()
	{
		var response = await Client.GetAsync($"api/chats/1/messages/-999");
		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

		var details = JsonConvert.DeserializeObject<ProblemDetails>(await response.Content.ReadAsStringAsync());
		Assert.NotNull(details);
		Assert.Equal(404, details.Status);
	}

	[Fact]
	public async Task Get_AllMessages_ShouldReturnAllMessagesInThatChat()
	{
		var chat = _registeredChats[0];
		var anotherChat = _registeredChats[1];
		var messages = new List<Message>
		{
			new Message { Chat = chat, Sender = _registeredUsers[2], Content = "test message" },
			new Message { Chat = chat, Sender = _registeredUsers[1], Content = "test message 2" },
			new Message { Chat = chat, Sender = _registeredUsers[2], Content = "test message 3" },
			new Message { Chat = anotherChat, Sender = _registeredUsers[0], Content = "message from another chat" },
		};
		DbContext.Messages.AddRange(messages);
		DbContext.SaveChanges();

		var messagesindb = DbContext.Messages.ToList();

		var response = await Client.GetAsync($"api/chats/{chat.Id}/messages");
		response.EnsureSuccessStatusCode();

		var data = JsonConvert.DeserializeObject<List<MessageDto>>(await response.Content.ReadAsStringAsync());
		Assert.NotNull(data);
		Assert.Collection(data,
			item1 => Assert.Equal("test message", item1.Contet),
			item2 => Assert.Equal("test message 2", item2.Contet),
			item3 => Assert.Equal("test message 3", item3.Contet));

	}

	[Fact]
	public async Task Post_ValidRequest_ShouldReturnCreatedMessage()
	{
		var chat = _registeredChats[0];
		var sender = _registeredUsers[1];
		var request = new CreateMessageRequest("test message");

		Client.DefaultRequestHeaders.Add("Authorization", sender.Id.ToString());
		var response = await Client.PostAsJsonAsync($"api/chats/{chat.Id}/messages", request);
		response.EnsureSuccessStatusCode();

		var data = JsonConvert.DeserializeObject<MessageDto>(await response.Content.ReadAsStringAsync());
		Assert.NotNull(data);
		Assert.Equal(request.Content, data.Contet);
	}

	[Fact]
	public async Task Delete_MessageExitsAndIsSender_ShouldReturnSuccess()
	{
		var chat = _registeredChats[0];
		var sender = _registeredUsers[1];
		var message = new Message { Chat = chat, Sender = sender, Content = "test message" };
		DbContext.Messages.Add(message);
		DbContext.SaveChanges();

		Client.DefaultRequestHeaders.Add("Authorization", sender.Id.ToString());
		var response = await Client.DeleteAsync($"api/chats/{chat.Id}/messages/{message.Id}");
		response.EnsureSuccessStatusCode();
	}


	[Fact]
	public async Task Delete_MessageExitsAndIsNotSender_ShouldReturnForbidden()
	{
		var chat = _registeredChats[0];
		var sender = _registeredUsers[1];
		var notSender = _registeredUsers[2];
		var message = new Message { Chat = chat, Sender = sender, Content = "test message" };
		DbContext.Messages.Add(message);
		DbContext.SaveChanges();

		Client.DefaultRequestHeaders.Add("Authorization", notSender.Id.ToString());
		var response = await Client.DeleteAsync($"api/chats/{chat.Id}/messages/{message.Id}");
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

		var details = JsonConvert.DeserializeObject<ProblemDetails>(await response.Content.ReadAsStringAsync());
		Assert.NotNull(details);
		Assert.Equal(403, details.Status);
	}
}
