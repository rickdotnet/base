using RickDotNet.Base;

namespace RickDotNet.Extensions.Base;

public static class ResultExtensions
{
    // this might have to go into the "naughty" namespace
    // public static bool IsSuccess<T>(this Result<T> result) => 
    //     result is Result<T>.Success;

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

    public static async Task<Result<TResult>> Select<T, TResult>(this Task<Result<T>> task, Func<T, Task<TResult>> onSuccess)
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

    public static async Task<Result<TResult>> Select<T, TResult>(this Result<T> result, Func<T, Task<TResult>> onSuccess)
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

    public static async Task<Result<TResult>> SelectMany<T, TResult>(this Result<T> result,
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

    public static void OnSuccess<T>(this Result<T> result, Action<T> onSuccess)
    {
        if (result is Result<T>.Success success)
            onSuccess(success.Value);
    }

    public static async Task OnSuccess<T>(this Result<T> result, Func<T, Task> onSuccess)
    {
        if (result is Result<T>.Success success)
            await onSuccess(success.Value);
    }

    public static async Task OnSuccess<T>(this Task<Result<T>> task, Func<T, Task> onSuccess)
    {
        var result = await task;
        if (result is Result<T>.Success success)
            await onSuccess(success.Value);
    }

    public static async Task OnSuccess<T>(this Task<Result<T>> task, Action<T> onSuccess)
    {
        var result = await task;
        if (result is Result<T>.Success success)
            onSuccess(success.Value);
    }

    public static void OnFailure<T>(this Result<T> result, Action<Exception> onFailure)
    {
        if (result is Result<T>.ExceptionalFailure exceptionalFailure)
            onFailure(exceptionalFailure.Exception);
    }


    public static async Task OnFailure<T>(this Result<T> result, Func<Exception, Task> onFailure)
    {
        if (result is Result<T>.ExceptionalFailure exceptionalFailure)
            await onFailure(exceptionalFailure.Exception);
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
}