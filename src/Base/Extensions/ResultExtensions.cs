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
    public static Result<TResult> Select<T, TResult>(this Result<T> result, Func<T, TResult> onSuccess)
    {
        var tryResult = Result.Try(() =>
        {
            return result switch
            {
                Result<T>.Success success => Result.Success(onSuccess(success.Value)),
                Result<T>.Error error => Result.Error<TResult>(error.ErrorMessage),
                Result<T>.Failure failure => Result.Failure<TResult>(failure.Exception),
                _ => throw new InvalidOperationException("Unknown Result type."),
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
    public static async Task<Result<TResult>> SelectAsync<T, TResult>(this Task<Result<T>> task, Func<T, Task<TResult>> onSuccess)
    {
        var tryResult = await Result.TryAsync(async () =>
        {
            var result = await task;

            return result switch
            {
                Result<T>.Success success => Result.Success(await onSuccess(success.Value)),
                Result<T>.Error error => Result.Error<TResult>(error.ErrorMessage),
                Result<T>.Failure failure => Result.Failure<TResult>(failure.Exception),
                _ => throw new InvalidOperationException("Unknown Result type."),
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
    public static async Task<Result<TResult>> SelectAsync<T, TResult>(this Result<T> result, Func<T, Task<TResult>> onSuccess)
    {
        var tryResult = await Result.TryAsync(async () =>
        {
            return result switch
            {
                Result<T>.Success success => Result.Success(await onSuccess(success.Value)),
                Result<T>.Error error => Result.Error<TResult>(error.ErrorMessage),
                Result<T>.Failure failure =>
                    Result.Failure<TResult>(failure.Exception),
                _ => throw new InvalidOperationException("Unknown Result type."),
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
                Result<T>.Error error => Result.Error<TResult>(error.ErrorMessage),
                Result<T>.Failure failure =>
                    Result.Failure<TResult>(failure.Exception),
                _ => throw new InvalidOperationException("Unknown Result type."),
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
                Result<T>.Error error => Result.Error<TResult>(error.ErrorMessage),
                Result<T>.Failure failure =>
                    Result.Failure<TResult>(failure.Exception),
                _ => throw new InvalidOperationException("Unknown Result type."),
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
    ///   Executes the <paramref name="onError"/> action if the result is a <see cref="Result{T}.Error"/> or a <see cref="Result{T}.Failure"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onError">The action to execute if the result is a non-success.</param>
    public static void OnError<T>(this Result<T> result, Action<string> onError)
    {
        switch (result)
        {
            case Result<T>.Error error:
                onError(error.ErrorMessage);
                break;
            case Result<T>.Failure failure:
                onError(failure.Exception.Message);
                break;
        }
    }

    /// <summary>
    ///   Executes the <paramref name="onError"/> action if the result is a <see cref="Result{T}.Error"/> or a <see cref="Result{T}.Failure"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onError">The action to execute if the result is a non-success.</param>
    public static Task OnErrorAsync<T>(this Result<T> result, Func<string, Task> onError)
    {
        return result switch
        {
            Result<T>.Error error => onError(error.ErrorMessage),
            Result<T>.Failure failure => onError(failure.Exception.Message),
            _ => Task.CompletedTask,
        };
    }

    /// <summary>
    ///  Executes the <paramref name="onFailure"/> action if the result is a <see cref="Result{T}.Failure"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onFailure">The action to execute if the result is a <see cref="Result{T}.Failure"/>.</param>
    public static void OnFailure<T>(this Result<T> result, Action<Exception> onFailure)
    {
        if (result is Result<T>.Failure failure)
            onFailure(failure.Exception);
    }

    /// <summary>
    ///  Executes the <paramref name="onFailure"/> action if the result is a <see cref="Result{T}.Failure"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onFailure">The action to execute if the result is a <see cref="Result{T}.Failure"/>.</param>
    public static Task OnFailureAsync<T>(this Result<T> result, Func<Exception, Task> onFailure)
    {
        if (result is Result<T>.Failure failure)
            return onFailure(failure.Exception);

        return Task.CompletedTask;
    }

    public static void Resolve<T>(this Result<T> result, Action<T> onSuccess, Action<string> onError)
    {
        switch (result)
        {
            case Result<T>.Success success:
                onSuccess(success.Value);
                break;
            case Result<T>.Error error:
                onError(error.ErrorMessage);
                break;
            case Result<T>.Failure exFail:
                onError(exFail.Exception.Message);
                break;
        }
    }

    public static Task ResolveAsync<T>(this Result<T> result, Func<T, Task> onSuccess, Func<string, Task> onError)
    {
        return result switch
        {
            Result<T>.Success success => onSuccess(success.Value),
            Result<T>.Error error => onError(error.ErrorMessage),
            Result<T>.Failure exFail => onError(exFail.Exception.Message),
            _ => Task.CompletedTask,
        };
    }

    public static void Resolve<T>(this Result<T> result, Action<T> onSuccess, Action<string> onError, Action<Exception> onFailure)
    {
        switch (result)
        {
            case Result<T>.Success success:
                onSuccess(success.Value);
                break;
            case Result<T>.Error error:
                onError(error.ErrorMessage);
                break;
            case Result<T>.Failure exFail:
                onFailure(exFail.Exception);
                break;
        }
    }

    public static Task ResolveAsync<T>(this Result<T> result, Func<T, Task> onSuccess, Func<string, Task> onError, Func<Exception, Task> onFailure)
    {
        return result switch
        {
            Result<T>.Success success => onSuccess(success.Value),
            Result<T>.Error error => onError(error.ErrorMessage),
            Result<T>.Failure exFail => onFailure(exFail.Exception),
            _ => Task.CompletedTask,
        };
    }

    public static T ValueOrDefault<T>(this Result<T> result, T defaultValue)
    {
        return result switch
        {
            Result<T>.Success success => success.Value,
            _ => defaultValue,
        };
    }
    
    public static T? ValueOrDefault<T>(this Result<T> result)
    {
        return result switch
        {
            Result<T>.Success success => success.Value,
            _ => default,
        };
    }

    public static async Task<T> ValueOrDefaultAsync<T>(this Task<Result<T>> resultTask, T defaultValue)
    {
        var result = await resultTask;
        return result switch
        {
            Result<T>.Success success => success.Value,
            _ => defaultValue,
        };
    }
    
    public static async Task<T?> ValueOrDefaultAsync<T>(this Task<Result<T>> resultTask)
    {
        var result = await resultTask;
        return result switch
        {
            Result<T>.Success success => success.Value,
            _ => default,
        };
    }

    public static async Task<T?> ValueOrDefaultAsync<T>(this ValueTask<Result<T>> resultTask, T? defaultValue = default)
    {
        var result = await resultTask;
        return result switch
        {
            Result<T>.Success success => success.Value,
            _ => defaultValue,
        };
    }
}