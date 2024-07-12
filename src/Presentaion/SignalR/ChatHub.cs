using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;
using SimpleSignalrChat.BusinessLogic.Services.Interfaces;
using SimpleSignalrChat.Presentaion.SignalR.Services;

namespace SimpleSignalrChat.Presentaion.SignalR;

public class ChatHub : Hub<IChatClient>
{
	private readonly IMessageService _messageService;
	private readonly IChatService _chatService;
	private readonly IUserService _userService;
	private readonly IHubGroupsManager<ChatHub> _hubGroupsManager;

	private int UserId { 
		get => Context.Items["userId"] as int? ?? 0;
		set => Context.Items["userId"] = value;
	}
	private async Task<int?> GetChatIdAsync()
	{
		string? chat = await _hubGroupsManager.GetUserGroupAsync(Context.ConnectionId);
		if (int.TryParse(chat, out int id))
		{
			return id;
		}
		return null;
	}

	public ChatHub(
		IMessageService messageService,
		IChatService chatService,
		IUserService userService,
		IHubGroupsManager<ChatHub> hubGroupsManager)
	{
		_messageService = messageService;
		_chatService = chatService;
		_userService = userService;
		_hubGroupsManager = hubGroupsManager;
	}

	public override async Task OnConnectedAsync()
	{
		var httpContext = Context.GetHttpContext()!;
		var authorizationHeader = httpContext.Request.Headers.Authorization;
		if (StringValues.IsNullOrEmpty(authorizationHeader))
		{
			await Clients.Caller.Error(nameof(OnConnectedAsync), "Authorization header not found");
			Context.Abort();
			return;
		}

		var userId = int.Parse(httpContext.Request.Headers["Authorization"].ToString());
		var userResult = await _userService.GetUserAsync(userId);

		userResult.Match(
			user => UserId = user.Id,
			error =>
			{
				Clients.Caller.Error(nameof(OnConnectedAsync), error.Message);
				Context.Abort();
			});
	}

	public async Task ConnectToChat(int chatId)
	{
		var currentChatId = await GetChatIdAsync();
		if (currentChatId is not null)
		{
			await Clients.Caller.ExitChat("Switching chats");
			await _hubGroupsManager.RemoveFromGroupAsync(currentChatId!.ToString(), Context.ConnectionId);
		}

		var chatResult = await _chatService.GetChatAsync(chatId);
		chatResult.Match(
			chat =>
			{
				Groups.AddToGroupAsync(Context.ConnectionId, chat.Id.ToString());
				Clients.Caller.ConnectToChat(chat);
			},
			error => Clients.Caller.Error(nameof(ConnectToChat), error.Message));

		await _hubGroupsManager.AddToGroupAsync(chatId.ToString(), Context.ConnectionId);

	}

	public async Task GetMessages()
	{
		int? chatId = await GetChatIdAsync();
		if (chatId is null)
		{
			await Clients.Caller.Error(nameof(GetMessages), "You are not connected to a chat");
			return;
		}

		var result = await _messageService.GetChatMessagesAsync(chatId.Value);
		result.Match(
			messages => Clients.Caller.GetMessages(messages),
			error => Clients.Caller.Error(nameof(GetMessages), error.Message));

	}

	public async Task SendMessage(string message)
	{
		int? chatId = await GetChatIdAsync();
		if (chatId is null)
		{
			await Clients.Caller.Error(nameof(SendMessage), "You are not connected to a chat");
			return;
		}

		var result = await _messageService.AddMessageAsync(chatId.Value, UserId, message);
		result.OnFailure(error =>
			Clients.Caller.Error(nameof(SendMessage), error.Message));
	}

	public async Task DeleteMessage(int id)
	{
		int? chatId = await GetChatIdAsync();
		if (chatId is null)
		{
			await Clients.Caller.Error(nameof(DeleteMessage), "You are not connected to a chat");
			return;
		}

		var result = await _messageService.DeleteMessageAsync(id, UserId);
		result.OnFailure(error =>
			Clients.Caller.Error(nameof(DeleteMessage), error.Message));
	}
}
