using Microsoft.AspNetCore.Mvc;
using SimpleSignalrChat.BusinessLogic.Exceptions.NotEnoughPrivilege;
using SimpleSignalrChat.BusinessLogic.Exceptions.NotFound;

namespace SimpleSignalrChat.Presentaion.Rest.ExceptionHandling;

public class ApiErrorMapper : IApiErrorMapper
{
	public ProblemDetails MapToProblemDetails(Exception exception)
	{
		return exception switch
		{
			NotFoundException => new ProblemDetails
			{
				Status = 404,
				Title = "The requested resource was not found.",
				Detail = exception.Message,
			},
			NotEnoughPrivilegeException => new ProblemDetails
			{
				Status = 403,
				Title = "You don't have enough privileges to perform this action.",
				Detail = exception.Message,
			},
			_ => new ProblemDetails
			{
				Status = 500,
				Title = "An error occurred while processing your request.",
			}
		};
	}
}
