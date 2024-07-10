using Microsoft.AspNetCore.Mvc;
using SimpleSignalrChat.BusinessLogic.Services.Interfaces;
using SimpleSignalrChat.Presentaion.Rest.ExceptionHandling;
using SimpleSignalrChat.Presentaion.Rest.Requests;
using System.ComponentModel.DataAnnotations;

namespace SimpleSignalrChat.Presentaion.Rest.Controllers;

[Route("api/chats/{chatId}/[controller]")]
[ApiController]
public class MessagesController : ControllerBase
{
	private readonly IMessageService _messageService;
	private readonly IApiErrorMapper _errorMapper;

	public MessagesController(IMessageService messageService, IApiErrorMapper errorMapper)
	{
		_messageService = messageService;
		_errorMapper = errorMapper;
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> Get(int chatId, int id)
	{
		var result = await _messageService.GetMessageAsync(id);
		return result.Map(Ok, _errorMapper.MapToActionResult);
	}

	[HttpGet]
	public async Task<IActionResult> Get(int chatId)
	{
		var result = await _messageService.GetChatMessagesAsync(chatId);
		return result.Map(Ok, _errorMapper.MapToActionResult);
	}

	[HttpPost]
	public async Task<IActionResult> Post(int chatId, CreateMessageRequest request, [FromHeader, Required] int userId)
	{
		var result = await _messageService.AddMessageAsync(chatId, userId, request.Content);
		return result.Map(
			message => CreatedAtAction(nameof(Get), new { chatId, id = message.Id }, message),
			_errorMapper.MapToActionResult);
	}

	[HttpDelete]
	public async Task<IActionResult> Delete(int chatId, int id, [FromHeader, Required] int userId)
	{
		var result = await _messageService.DeleteMessageAsync(id, userId);
		return result.Map(NoContent, _errorMapper.MapToActionResult);
	}
}
