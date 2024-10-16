namespace RickDotNet.Base;

public static class Result
{
    public static Result<T> Success<T>(T value) => new Result<T>.Success(value);
    public static Result<T> Failure<T>(string errorMessage) => new Result<T>.Failure(errorMessage);
    public static Result<T> Failure<T>(Exception error) => new Result<T>.ExceptionalFailure(error);

    public static Result<T> Try<T>(Func<T> func)
    {
        try
        {
            return Success(func());
        }
        catch (Exception e)
        {
            return Failure<T>(e);
        }
    }

    public static async Task<Result<T>> Try<T>(Func<Task<T>> func)
    {
        try
        {
            return Success(await func());
        }
        catch (Exception e)
        {
            return Failure<T>(e);
        }
    }

    public static async Task<Result<T>> Try<T>(Func<Task<Result<T>>> func)
    {
        try
        {
            return await func();
        }
        catch (Exception e)
        {
            return Failure<T>(e);
        }
    }

    public static Result<T> Try<T>(Func<Result<T>> func)
    {
        try
        {
            return func();
        }
        catch (Exception e)
        {
            return Failure<T>(e);
        }
    }

    public static Result<T> Try<T>(Func<Result<T>> func, Action<Exception> onCatch)
    {
        try
        {
            return func();
        }
        catch (Exception e)
        {
            onCatch(e);
            return Failure<T>(e);
        }
    }

    public static Result<T> Try<T>(Func<Result<T>> func, string errorMessage)
    {
        try
        {
            return func();
        }
        catch (Exception)
        {
            return Failure<T>(errorMessage);
        }
    }

    public static async Task<Result<T>> Try<T>(Func<Task<Result<T>>> func, string errorMessage)
    {
        try
        {
            return await func();
        }
        catch (Exception)
        {
            return Failure<T>(errorMessage);
        }
    }
}
public abstract record Result<T>
{
    public sealed record Success(T Value) : Result<T>;

    public sealed record Failure(string Error) : Result<T>;

    public sealed record ExceptionalFailure(Exception Exception) : Result<T>;

    public static implicit operator Result<T>(T value)
        => new Success(value);
    
    public static implicit operator Result<T>(Exception ex)
        => new ExceptionalFailure(ex);

    public static implicit operator bool(Result<T> result)
        => result is Success;

    public override string ToString() =>
        this switch
        {
            Success success => $"Success: {success.Value}",
            Failure failure => $"Failure: {failure.Error}",
            ExceptionalFailure exception => $"ExceptionFailure: {exception.Exception.Message}",
            _ => "Unknown Result"
        };
}