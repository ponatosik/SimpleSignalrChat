using Microsoft.AspNetCore.Mvc;
using SimpleSignalrChat.BusinessLogic.Services.Interfaces;
using SimpleSignalrChat.Presentaion.Rest.ExceptionHandling;
using SimpleSignalrChat.Presentaion.Rest.Requests;
using System.ComponentModel.DataAnnotations;

namespace SimpleSignalrChat.Presentaion.Rest.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
	private readonly IUserService _userService;
	private readonly IApiErrorMapper _errorMapper;

	public UsersController(IUserService userService, IApiErrorMapper resultMapper)
	{
		_userService = userService;
		_errorMapper = resultMapper;
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> Get(int id)
	{
		var result = await _userService.GetUserAsync(id);
		return result.Map(Ok, _errorMapper.MapToActionResult);
	}

	[HttpPost]
	public async Task<IActionResult> Post(CreateUserRequest request)
	{
		var result = await _userService.CreateUserAsync(request.Username);
		return result.Map(
			user => CreatedAtAction(nameof(Get), new { id = user.Id }, user),
			_errorMapper.MapToActionResult);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete([FromHeader, Required] int userId)
	{
		var result = await _userService.DeleteUserAsync(userId);
		return result.Map(NoContent, _errorMapper.MapToActionResult);
	}
}
