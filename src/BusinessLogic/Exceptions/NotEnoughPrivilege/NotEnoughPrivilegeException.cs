namespace SimpleSignalrChat.BusinessLogic.Exceptions.NotEnoughPrivilege;

public class NotEnoughPrivilegeException : Exception
{
	public string RequiredPrivilege { get; }
	public string Action { get; }

	public NotEnoughPrivilegeException(string requiredPrivilege, string action)
		: base(GetMessage(requiredPrivilege, action))
	{
		RequiredPrivilege = requiredPrivilege;
		Action = action;
	}

	private static string GetMessage(string requiredPrivilege, string action) =>
	 $"You must have {requiredPrivilege} privilege to perform this action: {action}";
}
