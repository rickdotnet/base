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
    /// <returns>A task representing the mapped result.</returns>
    public static async Task<Result<TResult>> SelectAsync<T, TResult>(this Result<T> result, Func<T, Task<TResult>> onSuccess)
    {
        var tryResult = await Result.TryAsync(async () =>
        {
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
    /// Maps the result of a task to a new result type asynchronously.
    /// </summary>
    /// <param name="task">The task containing the result to map.</param>
    /// <param name="onSuccess">The asynchronous function to map the result with.</param>
    /// <typeparam name="T">The type of the original result.</typeparam>
    /// <typeparam name="TResult">The type of the new result.</typeparam>
    /// <returns>A task representing the mapped result.</returns>
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
    /// Maps the result to a new result type using a function that returns a <see cref="Result{TResult}"/>.
    /// </summary>
    /// <param name="result">The result to map.</param>
    /// <param name="onSuccess">The function to map the result with, returning a new result.</param>
    /// <typeparam name="T">The type of the original result.</typeparam>
    /// <typeparam name="TResult">The type of the new result.</typeparam>
    /// <returns>The mapped result.</returns>
    public static Result<TResult> Bind<T, TResult>(this Result<T> result, Func<T, Result<TResult>> onSuccess)
    {
        var tryResult = Result.Try(() =>
        {
            return result switch
            {
                Result<T>.Success success => onSuccess(success.Value),
                Result<T>.Error error => Result.Error<TResult>(error.ErrorMessage),
                Result<T>.Failure failure => Result.Failure<TResult>(failure.Exception),
                _ => throw new InvalidOperationException("Unknown Result type."),
            };
        });

        return tryResult;
    }

    /// <summary>
    /// Maps the result to a new result type using a function that returns a <see cref="Task{Result{TResult}}"/>.
    /// </summary>
    /// <param name="result">The result to map.</param>
    /// <param name="onSuccess">The function to map the result with, returning a new result.</param>
    /// <typeparam name="T">The type of the original result.</typeparam>
    /// <typeparam name="TResult">The type of the new result.</typeparam>
    /// <returns>A task representing the mapped result.</returns>
    public static async Task<Result<TResult>> BindAsync<T, TResult>(this Result<T> result, Func<T, Task<Result<TResult>>> onSuccess)
    {
        var tryResult = await Result.TryAsync(async () =>
        {
            return result switch
            {
                Result<T>.Success success => await onSuccess(success.Value),
                Result<T>.Error error => Result.Error<TResult>(error.ErrorMessage),
                Result<T>.Failure failure => Result.Failure<TResult>(failure.Exception),
                _ => throw new InvalidOperationException("Unknown Result type."),
            };
        });

        return tryResult;
    }

    /// <summary>
    /// Maps the result of a task to a new result type using a function that returns a <see cref="Task{Result{TResult}}"/>.
    /// </summary>
    /// <param name="task">The task containing the result to map.</param>
    /// <param name="onSuccess">The function to map the result with, returning a new result.</param>
    /// <typeparam name="T">The type of the original result.</typeparam>
    /// <typeparam name="TResult">The type of the new result.</typeparam>
    /// <returns>A task representing the mapped result.</returns>
    public static async Task<Result<TResult>> BindAsync<T, TResult>(this Task<Result<T>> task, Func<T, Task<Result<TResult>>> onSuccess)
    {
        var tryResult = await Result.TryAsync(async () =>
        {
            var result = await task;

            return result switch
            {
                Result<T>.Success success => await onSuccess(success.Value),
                Result<T>.Error error => Result.Error<TResult>(error.ErrorMessage),
                Result<T>.Failure failure => Result.Failure<TResult>(failure.Exception),
                _ => throw new InvalidOperationException("Unknown Result type."),
            };
        });

        return tryResult;
    }

    /// <summary>
    /// Executes the <paramref name="onSuccess"/> action if the result is a <see cref="Result{T}.Success"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onSuccess">The action to execute if the result is a <see cref="Result{T}.Success"/>.</param>
    /// <typeparam name="T">The type of the result value.</typeparam>
    public static void OnSuccess<T>(this Result<T> result, Action<T> onSuccess)
    {
        if (result is Result<T>.Success success)
            onSuccess(success.Value);
    }

    /// <summary>
    /// Executes the <paramref name="onSuccess"/> action if the result is a <see cref="Result{T}.Success"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onSuccess">The action to execute if the result is a <see cref="Result{T}.Success"/>.</param>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <returns>A task representing the execution of the action, or a completed task if the result is not a success.</returns>
    public static Task OnSuccessAsync<T>(this Result<T> result, Func<T, Task> onSuccess)
    {
        if (result is Result<T>.Success success)
            return onSuccess(success.Value);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Executes the <paramref name="onError"/> action if the result is a <see cref="Result{T}.Error"/> or a <see cref="Result{T}.Failure"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onError">The action to execute if the result is an error.</param>
    /// <typeparam name="T">The type of the result value.</typeparam>
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
    /// Executes the <paramref name="onError"/> action if the result is a <see cref="Result{T}.Error"/> or a <see cref="Result{T}.Failure"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onError">The action to execute if the result is an error.</param>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <returns>A task representing the execution of the action, or a completed task if the result is a success.</returns>
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
    /// Executes the <paramref name="onFailure"/> action if the result is a <see cref="Result{T}.Failure"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onFailure">The action to execute if the result is a <see cref="Result{T}.Failure"/>, receiving the exception.</param>
    /// <typeparam name="T">The type of the result value.</typeparam>
    public static void OnFailure<T>(this Result<T> result, Action<Exception> onFailure)
    {
        if (result is Result<T>.Failure failure)
            onFailure(failure.Exception);
    }

    /// <summary>
    /// Executes the <paramref name="onFailure"/> action if the result is a <see cref="Result{T}.Failure"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="onFailure">The action to execute if the result is a <see cref="Result{T}.Failure"/>, receiving the exception.</param>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <returns>A task representing the execution of the action, or a completed task if the result is not a failure.</returns>
    public static Task OnFailureAsync<T>(this Result<T> result, Func<Exception, Task> onFailure)
    {
        if (result is Result<T>.Failure failure)
            return onFailure(failure.Exception);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Resolves the result by executing the appropriate action based on its state.
    /// </summary>
    /// <param name="result">The result to resolve.</param>
    /// <param name="onSuccess">The action to execute if the result is a <see cref="Result{T}.Success"/>.</param>
    /// <param name="onError">The action to execute if the result is a <see cref="Result{T}.Error"/> or <see cref="Result{T}.Failure"/>, receiving the error message.</param>
    /// <typeparam name="T">The type of the result value.</typeparam>
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

    /// <summary>
    /// Resolves the result by executing the appropriate action based on its state.
    /// </summary>
    /// <param name="result">The result to resolve.</param>
    /// <param name="onSuccess">The action to execute if the result is a <see cref="Result{T}.Success"/>.</param>
    /// <param name="onError">The action to execute if the result is a <see cref="Result{T}.Error"/> or <see cref="Result{T}.Failure"/>, receiving the error message.</param>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <returns>A task representing the execution of the appropriate action.</returns>
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

    /// <summary>
    /// Resolves the result by executing the appropriate action based on its state, with separate handling for failures.
    /// </summary>
    /// <param name="result">The result to resolve.</param>
    /// <param name="onSuccess">The action to execute if the result is a <see cref="Result{T}.Success"/>.</param>
    /// <param name="onError">The action to execute if the result is a <see cref="Result{T}.Error"/>, receiving the error message.</param>
    /// <param name="onFailure">The action to execute if the result is a <see cref="Result{T}.Failure"/>, receiving the exception.</param>
    /// <typeparam name="T">The type of the result value.</typeparam>
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

    /// <summary>
    /// Resolves the result by executing the appropriate action based on its state, with separate handling for failures.
    /// </summary>
    /// <param name="result">The result to resolve.</param>
    /// <param name="onSuccess">The action to execute if the result is a <see cref="Result{T}.Success"/>.</param>
    /// <param name="onError">The action to execute if the result is a <see cref="Result{T}.Error"/>, receiving the error message.</param>
    /// <param name="onFailure">The action to execute if the result is a <see cref="Result{T}.Failure"/>, receiving the exception.</param>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <returns>A task representing the execution of the appropriate action.</returns>
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

    /// <summary>
    /// Returns the value of the result from a task if it is a <see cref="Result{T}.Success"/>, otherwise returns the specified default value.
    /// </summary>
    /// <param name="resultTask">The task containing the result to check.</param>
    /// <param name="defaultValue">The default value to return if the result is not a <see cref="Result{T}.Success"/>.</param>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <returns>A task representing the result value if successful, otherwise the <paramref name="defaultValue"/>.</returns>
    public static async Task<T> ValueOrDefaultAsync<T>(this Task<Result<T>> resultTask, T defaultValue)
    {
        var result = await resultTask;
        return result switch
        {
            Result<T>.Success success => success.Value,
            _ => defaultValue,
        };
    }

    /// <summary>
    /// Returns the value of the result from a task if it is a <see cref="Result{T}.Success"/>, otherwise returns the default value for the type.
    /// </summary>
    /// <param name="resultTask">The task containing the result to check.</param>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <returns>A task representing the result value if successful, otherwise the default value for <typeparamref name="T"/>.</returns>
    public static async Task<T?> ValueOrDefaultAsync<T>(this Task<Result<T>> resultTask)
    {
        var result = await resultTask;
        return result switch
        {
            Result<T>.Success success => success.Value,
            _ => default,
        };
    }

    /// <summary>
    /// Returns the value of the result from a value task if it is a <see cref="Result{T}.Success"/>, otherwise returns the specified default value.
    /// </summary>
    /// <param name="resultTask">The value task containing the result to check.</param>
    /// <param name="defaultValue">The default value to return if the result is not a <see cref="Result{T}.Success"/>. Defaults to the type's default value.</param>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <returns>A task representing the result value if successful, otherwise the <paramref name="defaultValue"/>.</returns>
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
