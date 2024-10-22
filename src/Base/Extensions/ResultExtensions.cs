using RickDotNet.Base;

// ReSharper disable once CheckNamespace
namespace RickDotNet.Extensions.Base;

public static class ResultExtensions
{
    public static Result<TResult> Select<T, TResult>(this Result<T> result, Func<T, TResult> onSuccess)
    {
        var tryResult = Result.Try(() =>
        {
            return result switch
            {
                Result<T>.Success success => Result.Success(onSuccess(success.Value)),
                Result<T>.Failure fail => Result.Failure<TResult>(fail.Error),
                Result<T>.ExceptionalFailure exceptionalFailure =>
                    Result.Failure<TResult>(exceptionalFailure.Exception),
                _ => throw new InvalidOperationException("Unknown StoreResult type.")
            };
        });

        return tryResult;
    }

    public static async Task<Result<TResult>> SelectAsync<T, TResult>(this Task<Result<T>> task, Func<T, Task<TResult>> onSuccess)
    {
        var tryResult = await Result.Try(async () =>
        {
            var result = await task;

            return result switch
            {
                Result<T>.Success success => Result.Success(await onSuccess(success.Value)),
                Result<T>.Failure failure => Result.Failure<TResult>(failure.Error),
                Result<T>.ExceptionalFailure exceptionalFailure =>
                    Result.Failure<TResult>(exceptionalFailure.Exception),
                _ => throw new InvalidOperationException("Unknown StoreResult type.")
            };
        });

        return tryResult;
    }

    public static async Task<Result<TResult>> SelectAsync<T, TResult>(this Result<T> result, Func<T, Task<TResult>> onSuccess)
    {
        var tryResult = await Result.Try(async () =>
        {
            return result switch
            {
                Result<T>.Success success => Result.Success(await onSuccess(success.Value)),
                Result<T>.Failure failure => Result.Failure<TResult>(failure.Error),
                Result<T>.ExceptionalFailure exceptionalFailure =>
                    Result.Failure<TResult>(exceptionalFailure.Exception),
                _ => throw new InvalidOperationException("Unknown StoreResult type.")
            };
        });

        return tryResult;
    }

    public static async Task<Result<TResult>> SelectManyAsync<T, TResult>(this Result<T> result,
        Func<T, Task<Result<TResult>>> onSuccess)
    {
        var tryResult = await Result.Try(async () =>
        {
            return result switch
            {
                Result<T>.Success success => await onSuccess(success.Value),
                Result<T>.Failure failure => Result.Failure<TResult>(failure.Error),
                Result<T>.ExceptionalFailure exceptionalFailure =>
                    Result.Failure<TResult>(exceptionalFailure.Exception),
                _ => throw new InvalidOperationException("Unknown StoreResult type.")
            };
        });

        return tryResult;
    }

    public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> onSuccess)
    {
        if (result is Result<T>.Success success)
            onSuccess(success.Value);

        return result;
    }

    public static async Task<Result<T>> OnSuccessAsync<T>(this Result<T> result, Func<T, Task> onSuccess)
    {
        if (result is Result<T>.Success success)
            await onSuccess(success.Value);

        return result;
    }

    public static Result<T> OnFailure<T>(this Result<T> result, Action<Exception> onFailure)
    {
        if (result is Result<T>.ExceptionalFailure exceptionalFailure)
            onFailure(exceptionalFailure.Exception);

        return result;
    }


    public static async Task<Result<T>> OnFailureAsync<T>(this Result<T> result, Func<Exception, Task> onFailure)
    {
        if (result is Result<T>.ExceptionalFailure exceptionalFailure)
            await onFailure(exceptionalFailure.Exception);
        return result;
    }

    public static void Resolve<T>(this Result<T> result, Action<T> onSuccess, Action<string>? onFailure = null,
        Action<Exception>? onException = null)
    {
        switch (result)
        {
            case Result<T>.Success success:
                onSuccess(success.Value);
                break;
            case Result<T>.Failure failure when onFailure != null:
                onFailure(failure.Error);
                break;
            case Result<T>.ExceptionalFailure exFail when onException != null:
                onException(exFail.Exception);
                break;
        }
    }

    public static T? ValueOrDefault<T>(this Result<T> result, T? defaultValue = default)
    {
        return result switch
        {
            Result<T>.Success success => success.Value,
            _ => defaultValue
        };
    }
}