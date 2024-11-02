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
        var tryResult = await Result.TryAsync(async () =>
        {
            var result = await task;

            return result switch
            {
                Result<T>.Success success => Result.Success(await onSuccess(success.Value)),
                Result<T>.Failure failure => Result.Failure<TResult>(failure.Error),
                Result<T>.ExceptionalFailure exceptionalFailure => Result.Failure<TResult>(exceptionalFailure.Exception),
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
        var tryResult = await Result.TryAsync(async () =>
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
        var tryResult = await Result.TryAsync(async () =>
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
    public static void OnSuccess<T>(this Result<T> result, Action<T> onSuccess)
    {
        if (result is Result<T>.Success success)
            onSuccess(success.Value);
    }

    /// <summary>
    ///  Executes the <paramref name="onSuccess"/> action if the result is a <see cref="Result{T}.Success"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onSuccess">The action to execute if the result is a <see cref="Result{T}.Success"/>.</param>
    public static Task OnSuccessAsync<T>(this Result<T> result, Func<T, Task> onSuccess)
    {
        if (result is Result<T>.Success success)
            return onSuccess(success.Value);

        return Task.CompletedTask;
    }

    /// <summary>
    ///   Executes the <paramref name="onError"/> action if the result is a <see cref="Result{T}.Failure"/> or a <see cref="Result{T}.ExceptionalFailure"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onError">The action to execute if the result is a non-success.</param>
    public static void OnError<T>(this Result<T> result, Action<string> onError)
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
    }

    /// <summary>
    ///   Executes the <paramref name="onError"/> action if the result is a <see cref="Result{T}.Failure"/> or a <see cref="Result{T}.ExceptionalFailure"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onError">The action to execute if the result is a non-success.</param>
    public static Task OnErrorAsync<T>(this Result<T> result, Func<string, Task> onError)
    {
        return result switch
        {
            Result<T>.Failure failure => onError(failure.Error),
            Result<T>.ExceptionalFailure exceptionalFailure => onError(exceptionalFailure.Exception.Message),
            _ => Task.CompletedTask
        };
    }

    /// <summary>
    ///  Executes the <paramref name="onFailure"/> action if the result is a <see cref="Result{T}.Failure"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onFailure">The action to execute if the result is a <see cref="Result{T}.Failure"/>.</param>
    public static void OnFailure<T>(this Result<T> result, Action<string> onFailure)
    {
        if (result is Result<T>.Failure failure)
            onFailure(failure.Error);
    }

    /// <summary>
    ///  Executes the <paramref name="onFailure"/> action if the result is a <see cref="Result{T}.Failure"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onFailure">The action to execute if the result is a <see cref="Result{T}.Failure"/>.</param>
    public static Task OnFailureAsync<T>(this Result<T> result, Func<string, Task> onFailure)
    {
        if (result is Result<T>.Failure failure)
            return onFailure(failure.Error);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Executes the <paramref name="onExceptionalFailure"/> action if the result is a <see cref="Result{T}.ExceptionalFailure"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onExceptionalFailure">The action to execute if the result is a <see cref="Result{T}.ExceptionalFailure"/>.</param>
    public static void OnExceptionalFailure<T>(this Result<T> result, Action<Exception> onExceptionalFailure)
    {
        if (result is Result<T>.ExceptionalFailure exceptionalFailure)
            onExceptionalFailure(exceptionalFailure.Exception);
    }

    /// <summary>
    /// Executes the <paramref name="onExceptionalFailure"/> action if the result is a <see cref="Result{T}.ExceptionalFailure"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onExceptionalFailure">The action to execute if the result is a <see cref="Result{T}.ExceptionalFailure"/>.</param>
    public static Task OnExceptionalFailureAsync<T>(this Result<T> result, Func<Exception, Task> onExceptionalFailure)
    {
        if (result is Result<T>.ExceptionalFailure exceptionalFailure)
            return onExceptionalFailure(exceptionalFailure.Exception);

        return Task.CompletedTask;
    }

    public static void Resolve<T>(this Result<T> result, Action<T> onSuccess, Action<string> onError)
    {
        switch (result)
        {
            case Result<T>.Success success:
                onSuccess(success.Value);
                break;
            case Result<T>.Failure failure:
                onError(failure.Error);
                break;
            case Result<T>.ExceptionalFailure exFail:
                onError(exFail.Exception.Message);
                break;
        }
    }

    public static Task ResolveAsync<T>(this Result<T> result, Func<T, Task> onSuccess, Func<string, Task> onError)
    {
        return result switch
        {
            Result<T>.Success success => onSuccess(success.Value),
            Result<T>.Failure failure => onError(failure.Error),
            Result<T>.ExceptionalFailure exFail => onError(exFail.Exception.Message),
            _ => Task.CompletedTask
        };
    }

    public static void Resolve<T>(this Result<T> result, Action<T> onSuccess, Action<string> onFailure, Action<Exception> onException)
    {
        switch (result)
        {
            case Result<T>.Success success:
                onSuccess(success.Value);
                break;
            case Result<T>.Failure failure:
                onFailure(failure.Error);
                break;
            case Result<T>.ExceptionalFailure exFail:
                onException(exFail.Exception);
                break;
        }
    }

    public static Task ResolveAsync<T>(this Result<T> result, Func<T, Task> onSuccess, Func<string, Task> onFailure, Func<Exception, Task> onException)
    {
        return result switch
        {
            Result<T>.Success success => onSuccess(success.Value),
            Result<T>.Failure failure => onFailure(failure.Error),
            Result<T>.ExceptionalFailure exFail => onException(exFail.Exception),
            _ => Task.CompletedTask
        };
    }

    public static T? ValueOrDefault<T>(this Result<T> result, T? defaultValue = default)
    {
        return result switch
        {
            Result<T>.Success success => success.Value,
            _ => defaultValue
        };
    }

    public static async Task<T?> ValueOrDefaultAsync<T>(this Task<Result<T>> resultTask, T? defaultValue = default)
    {
        var result = await resultTask;
        return result switch
        {
            Result<T>.Success success => success.Value,
            _ => defaultValue
        };
    }

    public static async Task<T?> ValueOrDefaultAsync<T>(this ValueTask<Result<T>> resultTask, T? defaultValue = default)
    {
        var result = await resultTask;
        return result switch
        {
            Result<T>.Success success => success.Value,
            _ => defaultValue
        };
    }
}