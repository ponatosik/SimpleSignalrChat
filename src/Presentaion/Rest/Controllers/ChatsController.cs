using Microsoft.AspNetCore.Mvc;
using SimpleSignalrChat.BusinessLogic.Services.Interfaces;
using SimpleSignalrChat.Presentaion.Rest.ExceptionHandling;
using SimpleSignalrChat.Presentaion.Rest.Requests;
using System.ComponentModel.DataAnnotations;

namespace SimpleSignalrChat.Presentaion.Rest.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatsController : ControllerBase
{
	private readonly IChatService _chatService;
	private readonly IApiErrorMapper _errorMapper;

	public ChatsController(IChatService chatService, IApiErrorMapper errorMapper)
	{
		_chatService = chatService;
		_errorMapper = errorMapper;
	}

	[HttpGet]
	public async Task<IActionResult> Get([FromQuery] string? filter, [FromQuery] int? adminId)
	{
		var result = await _chatService.GetAllChatsAsync(filter, adminId);
		return result.Map(Ok, _errorMapper.MapToActionResult);
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> Get(int id)
	{
		var result = await _chatService.GetChatAsync(id);
		return result.Map(Ok, _errorMapper.MapToActionResult);
	}

	[HttpPost]
	public async Task<IActionResult> Post(
		CreateChatRequest request,
		[FromHeader(Name = "Authorization"), Required] int userId)
	{
		var result = await _chatService.CreateChatAsync(userId, request.ChatName);
		return result.Map(
			chat => CreatedAtAction(nameof(Get), new { id = chat.Id }, chat),
			_errorMapper.MapToActionResult);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Put(
		CreateChatRequest request,
		int id,
		[FromHeader(Name = "Authorization"), Required] int userId)
	{
		var result = await _chatService.UpdateChatAsync(id, request.ChatName, userId);
		return result.Map(Ok, _errorMapper.MapToActionResult);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(
		int id, 
		[FromHeader(Name = "Authorization"), Required] int userId)
	{
		var result = await _chatService.DeleteChatAsync(id, userId);
		return result.Map(NoContent, _errorMapper.MapToActionResult);
	}
}
