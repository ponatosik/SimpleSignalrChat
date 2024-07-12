namespace SimpleSignalrChat.BusinessLogic.Abstractions;

public class Result<T>
{
	public bool IsSuccess { get; }
	public bool IsFailure => !IsSuccess;
	public T? Value { get; }
	public Exception? Error { get; }

	private Result(bool isSuccess, T? value, Exception? error)
	{
		IsSuccess = isSuccess;
		Value = value;
		Error = error;
	}

	public void Match(Action<T> onSuccess, Action<Exception> onFailure)
	{
		if (IsSuccess)
		{
			onSuccess(Value!);
		}
		else
		{
			onFailure(Error!);
		}
	}
	public R Map<R>(Func<T, R> onSuccess, Func<Exception, R> onFailure)
	{
		if (IsSuccess)
		{
			return onSuccess(Value!);
		}
		else
		{
			return onFailure(Error!);
		}
	}
	public void OnSuccess(Action<T> action)
	{
		if (IsSuccess)
		{
			action(Value!);
		}
	}
	public void OnFailure(Action<Exception> action)
	{
		if (IsFailure)
		{
			action(Error!);
		}
	}

	public static Result<T> Success(T value) => new Result<T>(true, value, null);
	public static Result<T> Failure(Exception error) => new Result<T>(false, default, error);

	public static implicit operator Result<T>(T value) => Success(value);
	public static implicit operator Result<T>(Exception error) => Failure(error);
}

public class Result
{
	public bool IsSuccess { get; }
	public bool IsFailure => !IsSuccess;
	public Exception? Error { get; }

	private Result(bool isSuccess, Exception? error)
	{
		IsSuccess = isSuccess;
		Error = error;
	}

	public static Result Success => new Result(true, null);
	public static Result Failure(Exception error) => new Result(false, error);

	public R Map<R>(Func<R> onSuccess, Func<Exception, R> onFailure)
	{
		if (IsSuccess)
		{
			return onSuccess();
		}
		else
		{
			return onFailure(Error!);
		}
	}
	public void OnSuccess(Action action)
	{
		if (IsSuccess)
		{
			action();
		}
	}
	public void OnFailure(Action<Exception> action)
	{
		if (IsFailure)
		{
			action(Error!);
		}
	}


	public static implicit operator Result(Exception error) => Failure(error);
}
