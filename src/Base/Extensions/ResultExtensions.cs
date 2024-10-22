using RickDotNet.Base;

// ReSharper disable once CheckNamespace
namespace RickDotNet.Extensions.Base;

public static class ResultExtensions
{
    /// <summary>
    /// Maps the result to a new result type.
    /// </summary>
    /// <param name="result">The result to map.</param>
    /// <param name="onSuccess">The function to map the result with.</param>
    /// <typeparam name="T">The type of the original result.</typeparam>
    /// <typeparam name="TResult">The type of the new result.</typeparam>
    /// <returns>The mapped result.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the result is an unknown Result type.</exception>
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
                _ => throw new InvalidOperationException("Unknown Result type.")
            };
        });

        return tryResult;
    }

    /// <summary>
    /// Maps the result to a new result type.
    /// </summary>
    /// <param name="task">The result to map.</param>
    /// <param name="onSuccess">The function to map the result with.</param>
    /// <typeparam name="T">The type of the original result.</typeparam>
    /// <typeparam name="TResult">The type of the new result.</typeparam>
    /// <returns>The mapped result.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the result is an unknown Result type.</exception>
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

    /// <summary>
    /// Maps the result to a new result type.
    /// </summary>
    /// <param name="result">The result to map.</param>
    /// <param name="onSuccess">The function to map the result with.</param>
    /// <typeparam name="T">The type of the original result.</typeparam>
    /// <typeparam name="TResult">The type of the new result.</typeparam>
    /// <returns>The mapped result.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the result is an unknown Result type.</exception>
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
    
    public static Result<TResult> SelectMany<T, TResult>(this Result<T> result,
        Func<T, Result<TResult>> onSuccess)
    {
        var tryResult = Result.Try(() =>
        {
            return result switch
            {
                Result<T>.Success success => onSuccess(success.Value),
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

    /// <summary>
    ///  Executes the <paramref name="onSuccess"/> action if the result is a <see cref="Result{T}.Success"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onSuccess">The action to execute if the result is a <see cref="Result{T}.Success"/>.</param>
    /// <returns>The original result.</returns>
    public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> onSuccess)
    {
        if (result is Result<T>.Success success)
            onSuccess(success.Value);

        return result;
    }

    /// <summary>
    ///  Executes the <paramref name="onSuccess"/> action if the result is a <see cref="Result{T}.Success"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onSuccess">The action to execute if the result is a <see cref="Result{T}.Success"/>.</param>
    /// <returns>The original result.</returns>
    public static async Task<Result<T>> OnSuccessAsync<T>(this Result<T> result, Func<T, Task> onSuccess)
    {
        if (result is Result<T>.Success success)
            await onSuccess(success.Value);

        return result;
    }

    /// <summary>
    ///   Executes the <paramref name="onError"/> action if the result is a <see cref="Result{T}.Failure"/> or a <see cref="Result{T}.ExceptionalFailure"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onError">The action to execute if the result is a non-success.</param>
    /// <returns>The original result.</returns>
    public static Result<T> OnError<T>(this Result<T> result, Action<string> onError)
    {
        switch (result)
        {
            case Result<T>.Failure failure:
                onError(failure.Error);
                break;
            case Result<T>.ExceptionalFailure exceptionalFailure:
                onError(exceptionalFailure.Exception.Message);
                break;
        }

        return result;
    }

    /// <summary>
    ///   Executes the <paramref name="onError"/> action if the result is a <see cref="Result{T}.Failure"/> or a <see cref="Result{T}.ExceptionalFailure"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onError">The action to execute if the result is a non-success.</param>
    /// <returns>The original result.</returns>
    public static async Task<Result<T>> OnErrorAsync<T>(this Result<T> result, Func<string, Task> onError)
    {
        switch (result)
        {
            case Result<T>.Failure failure:
                await onError(failure.Error);
                break;
            case Result<T>.ExceptionalFailure exceptionalFailure:
                await onError(exceptionalFailure.Exception.Message);
                break;
        }

        return result;
    }

    /// <summary>
    ///  Executes the <paramref name="onFailure"/> action if the result is a <see cref="Result{T}.Failure"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onFailure">The action to execute if the result is a <see cref="Result{T}.Failure"/>.</param>
    /// <returns>The original result.</returns>
    public static Result<T> OnFailure<T>(this Result<T> result, Action<string> onFailure)
    {
        if (result is Result<T>.Failure failure)
            onFailure(failure.Error);

        return result;
    }

    /// <summary>
    ///  Executes the <paramref name="onFailure"/> action if the result is a <see cref="Result{T}.Failure"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onFailure">The action to execute if the result is a <see cref="Result{T}.Failure"/>.</param>
    /// <returns>The original result.</returns>
    public static async Task<Result<T>> OnFailureAsync<T>(this Result<T> result, Func<string, Task> onFailure)
    {
        if (result is Result<T>.Failure failure)
            await onFailure(failure.Error);

        return result;
    }

    /// <summary>
    /// Executes the <paramref name="onExceptionalFailure"/> action if the result is a <see cref="Result{T}.ExceptionalFailure"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onExceptionalFailure">The action to execute if the result is a <see cref="Result{T}.ExceptionalFailure"/>.</param>
    /// <returns>The original result.</returns>
    public static Result<T> OnExceptionalFailure<T>(this Result<T> result, Action<Exception> onExceptionalFailure)
    {
        if (result is Result<T>.ExceptionalFailure exceptionalFailure)
            onExceptionalFailure(exceptionalFailure.Exception);

        return result;
    }

    /// <summary>
    /// Executes the <paramref name="onExceptionalFailure"/> action if the result is a <see cref="Result{T}.ExceptionalFailure"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onExceptionalFailure">The action to execute if the result is a <see cref="Result{T}.ExceptionalFailure"/>.</param>
    /// <returns>The original result.</returns>
    public static async Task<Result<T>> OnExceptionalFailureAsync<T>(this Result<T> result, Func<Exception, Task> onExceptionalFailure)
    {
        if (result is Result<T>.ExceptionalFailure exceptionalFailure)
            await onExceptionalFailure(exceptionalFailure.Exception);
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