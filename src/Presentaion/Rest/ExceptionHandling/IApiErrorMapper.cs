using Microsoft.AspNetCore.Mvc;
using SimpleSignalrChat.BusinessLogic.Abstractions;

namespace SimpleSignalrChat.Presentaion.Rest.ExceptionHandling;

public interface IApiErrorMapper
{
	public IActionResult MapToActionResult(Exception exception) 
	{
		ProblemDetails details = MapToProblemDetails(exception);
		return new ObjectResult(details) 
		{ 
			ContentTypes = { "application/problem+json" },
			StatusCode = details.Status,
		};
	}

	public ProblemDetails MapToProblemDetails (Exception exception);
}
