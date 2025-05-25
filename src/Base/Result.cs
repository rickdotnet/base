namespace RickDotNet.Base;

public struct VoidResult
{
    public static readonly VoidResult Default = new();
}

public static class Result
{
    public static Result<VoidResult> Success() => new Result<VoidResult>.Success(VoidResult.Default);
    public static Result<VoidResult> Error(string error) => new Result<VoidResult>.Error(error);
    public static Result<VoidResult> Failure(Exception failure) => new Result<VoidResult>.Failure(failure);
    public static Result<T> Success<T>(T value) => new Result<T>.Success(value);
    public static Result<T> Error<T>(string errorMessage) => new Result<T>.Error(errorMessage);
    public static Result<T> Error<T>(Exception failure) => new Result<T>.Error(failure.Message);
    public static Result<T> Failure<T>(Exception failure) => new Result<T>.Failure(failure);

    public static Result<VoidResult> Try(Action action)
    {
        try
        {
            action();
            return Success(VoidResult.Default);
        }
        catch (Exception e)
        {
            return Failure<VoidResult>(e);
        }
    }
    public static async Task<Result<VoidResult>> TryAsync(Func<Task> func)
    {
        try
        {
            await func();
            return Success(VoidResult.Default);
        }
        catch (Exception e)
        {
            return Failure<VoidResult>(e);
        }
    }
    
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

   public static async Task<Result<T>> TryAsync<T>(Func<Task<T>> func)
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

    public static async Task<Result<T>> TryAsync<T>(Func<Task<Result<T>>> func)
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
        catch
        {
            return Error<T>(errorMessage);
        }
    }

    public static async Task<Result<T>> TryAsync<T>(Func<Task<Result<T>>> func, string errorMessage)
    {
        try
        {
            return await func();
        }
        catch
        {
            return Error<T>(errorMessage);
        }
    }
}

public abstract record Result<T>
{
    public sealed record Success(T Value) : Result<T>;

    public sealed record Error(string ErrorMessage) : Result<T>;

    public sealed record Failure(Exception Exception) : Result<T>;

    public static implicit operator Result<T>(T value)
        => new Success(value);

    public static implicit operator Result<T>(Exception ex)
        => new Failure(ex);

    public static implicit operator bool(Result<T> result)
        => result is Success;

    public override string ToString() =>
        this switch
        {
            Success success => $"Success: {success.Value}",
            Error error => $"Error: {error}",
            Failure exception => $"Failure: {exception.Exception.Message}",
            _ => "Unknown Result"
        };
}