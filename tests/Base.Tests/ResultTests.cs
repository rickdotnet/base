using RickDotNet.Base;
using RickDotNet.Extensions.Base;

namespace Base.Tests;

public class ResultTests
{
    [Fact]
    public void ImplicitConversion_FromValue_ReturnsSuccessResult()
    {
        Result<int> result = 420;

        Assert.True(result);
        var successResult = result as Result<int>.Success;
        Assert.NotNull(successResult);
        Assert.Equal(420, successResult.Value);
    }

    [Fact]
    public void ImplicitConversion_FromException_ReturnsFailureResult()
    {
        var exception = new InvalidOperationException("BOOM!");

        Result<int> result = exception;
        Assert.False(result);

        var failure = result as Result<int>.Failure;
        Assert.NotNull(failure);
        Assert.Equal(exception.Message, failure.Exception.Message);
    }

    private readonly Func<Result<int>> faultyFunc = () => throw new InvalidOperationException();

    [Fact]
    public void Try_WithCatch_HandlerIsCalledOnException()
    {
        var exceptionHandled = false;

        var result = Result.Try(faultyFunc, _ => exceptionHandled = true);

        Assert.False(result);
        Assert.True(exceptionHandled);
    }

    [Fact]
    public void Try_WithErrorMessage_ReturnsErrorWithMessage()
    {
        var result = Result.Try(faultyFunc, "Error");

        Assert.False(result);
        Assert.Equal("Error", ((Result<int>.Error)result).ErrorMessage);
    }

    [Fact]
    public void Try_SyncOverloads_ReturnsSuccessOnSuccess()
    {
        var result1 = Result.Try(() => 5);
        Assert.True(result1);
        Assert.Equal(5, result1.ValueOrDefault());

        var result2 = Result.Try(() => Console.WriteLine("Cool"));
        Assert.True(result2);
    }

    [Fact]
    public async Task Try_AsyncOverloads_ReturnsSuccessOnSuccess()
    {
        var result1 = await Result.TryAsync(async () => await Task.FromResult(5));
        Assert.True(result1);
        Assert.Equal(5, result1.ValueOrDefault());

        var result2 = await Result.TryAsync(async () => await Task.CompletedTask);
        Assert.True(result2);

        var result3 = await Result.TryAsync(async () => await Task.FromException<int>(new Exception("Oops")));
        Assert.False(result3);
        result3.OnError(error => Assert.Equal("Oops", error));
    }

    [Fact]
    public void OnFailure_ExecutesActionOnException()
    {
        var wasCalled = false;

        var result = Result.Failure<int>(new InvalidOperationException("Oops"));

        result.OnFailure(ex => wasCalled = ex.Message == "Oops");
        Assert.True(wasCalled);
    }

    [Fact]
    public void Select_Success_ReturnsTransformedResult()
    {
        var result = Result.Success(5);
        var transformedResult = result.Select(x => x * 2);

        Assert.Equal(10, transformedResult.ValueOrDefault());
    }

    [Fact]
    public void Select_Failure_ReturnsFailure()
    {
        var exception = new Exception("Failure");
        var result = Result.Failure<int>(exception);
        var transformedResult = result.Select(x => x * 2);

        Assert.Equal(exception.Message, ((Result<int>.Failure)transformedResult).Exception.Message);
    }

    [Fact]
    public async Task SelectAsync_Success_ReturnsTransformedResult()
    {
        var result = Result.Success(5);
        var transformedResult = await result.SelectAsync(async x => await Task.FromResult(x * 2));

        Assert.Equal(10, transformedResult.ValueOrDefault());
    }

    [Fact]
    public async Task SelectAsync_Failure_ReturnsFailure()
    {
        var exception = new Exception("Failure");
        var result = Result.Failure<int>(exception);
        var transformedResult = await result.SelectAsync(async x => await Task.FromResult(x * 2));

        Assert.Equal(exception.Message, ((Result<int>.Failure)transformedResult).Exception.Message);
    }

    [Fact]
    public async Task SelectAsync_TaskResult_Success_ChainsTogether()
    {
        var result = Task.FromResult(Result.Success(5));

        var chainedResult = await result.SelectAsync(async x => await Task.FromResult(x * 2));

        Assert.True(chainedResult);
        Assert.Equal(10, chainedResult.ValueOrDefault());
    }

    [Fact]
    public async Task SelectAsync_TaskResult_Failure_SkipsOnSuccess()
    {
        var exception = new Exception("Failure");
        var result = Task.FromResult(Result.Failure<int>(exception));
        var wasCalled = false;
        var transformedResult = await result.SelectAsync(_ => Task.FromResult(wasCalled = true));

        Assert.False(transformedResult);
        Assert.False(wasCalled);
        Assert.Equal(exception.Message, ((Result<bool>.Failure)transformedResult).Exception.Message);
    }

    [Fact]
    public void Resolve_WithOnSuccessAndOnError_CallsOnSuccessForSuccessResult()
    {
        var result = Result.Success(100);
        var successCalled = false;
        var errorCalled = false;

        result.Resolve(
            onSuccess: val => successCalled = val == 100,
            onError: _ => errorCalled = true
        );

        Assert.True(successCalled);
        Assert.False(errorCalled);
    }

    [Fact]
    public void Resolve_WithOnSuccessAndOnError_CallsOnErrorForFailureResult()
    {
        var exception = new Exception("Failure");
        var result = Result.Failure<int>(exception);
        var successCalled = false;
        var errorCalled = false;

        result.Resolve(
            onSuccess: _ => successCalled = true,
            onError: err => errorCalled = err == exception.Message
        );

        Assert.False(successCalled);
        Assert.True(errorCalled);
    }

    [Fact]
    public void Resolve_WithOnSuccessAndOnError_CallsOnErrorForFailure()
    {
        var exception = new InvalidOperationException("Failure");
        var result = Result.Failure<int>(exception);
        var successCalled = false;
        var errorCalled = false;

        result.Resolve(
            onSuccess: _ => successCalled = true,
            onError: err => errorCalled = err == exception.Message
        );

        Assert.False(successCalled);
        Assert.True(errorCalled);
    }

    [Fact]
    public void Resolve_WithAllThree_CallsOnSuccessForSuccessResult()
    {
        var result = Result.Success(200);
        var successCalled = false;
        var failureCalled = false;
        var exceptionCalled = false;

        result.Resolve(
            onSuccess: val => successCalled = val == 200,
            onError: _ => failureCalled = true,
            onFailure: _ => exceptionCalled = true
        );

        Assert.True(successCalled);
        Assert.False(failureCalled);
        Assert.False(exceptionCalled);
    }

    [Fact]
    public void Resolve_WithAllThree_CallsOnFailureForFailureResult()
    {
        var exception = new Exception("Failure");
        var result = Result.Failure<int>(exception);
        var successCalled = false;
        var errorCalled = false;
        var failureCalled = false;

        result.Resolve(
            onSuccess: _ => successCalled = true,
            onError: err => errorCalled = err == exception.Message,
            onFailure: _ => failureCalled = true
        );

        Assert.False(successCalled);
        Assert.False(errorCalled);
        Assert.True(failureCalled);
    }

    [Fact]
    public void Resolve_WithAllThree_CallsOnExceptionForFailure()
    {
        var exception = new InvalidOperationException("BOOM!");
        var result = Result.Failure<int>(exception);
        var successCalled = false;
        var failureCalled = false;
        var exceptionCalled = false;

        result.Resolve(
            onSuccess: _ => successCalled = true,
            onError: _ => failureCalled = true,
            onFailure: ex => exceptionCalled = ex is InvalidOperationException
        );

        Assert.False(successCalled);
        Assert.False(failureCalled);
        Assert.True(exceptionCalled);
    }

    [Fact]
    public async Task ResolveAsync_WithOnSuccessAndOnError_CallsOnSuccessForSuccessResult()
    {
        var result = Result.Success(100);
        var successCalled = false;
        var errorCalled = false;

        await result.ResolveAsync(
            onSuccess: val => Task.FromResult(successCalled = val == 100),
            onError: _ => Task.FromResult(errorCalled = true)
        );

        Assert.True(successCalled);
        Assert.False(errorCalled);
    }

    [Fact]
    public async Task ResolveAsync_WithOnSuccessAndOnError_CallsOnErrorForFailureResult()
    {
        var exception = new Exception("Error");
        var result = Result.Error<int>(exception);
        var successCalled = false;
        var errorCalled = false;

        await result.ResolveAsync(
            onSuccess: _ => Task.FromResult(successCalled = true),
            onError: err => Task.FromResult(errorCalled = err == exception.Message)
        );

        Assert.False(successCalled);
        Assert.True(errorCalled);
    }

    [Fact]
    public async Task ResolveAsync_WithOnSuccessAndOnError_CallsOnErrorForFailure()
    {
        var exception = new InvalidOperationException("BOOM!");
        var result = Result.Failure<int>(exception);
        var successCalled = false;
        var errorCalled = false;

        await result.ResolveAsync(
            onSuccess: _ => Task.FromResult(successCalled = true),
            onError: err => Task.FromResult(errorCalled = err == exception.Message)
        );

        Assert.False(successCalled);
        Assert.True(errorCalled);
    }

    [Fact]
    public async Task ResolveAsync_WithAllThree_CallsOnSuccessForSuccessResult()
    {
        var result = Result.Success(200);
        var successCalled = false;
        var failureCalled = false;
        var exceptionCalled = false;

        await result.ResolveAsync(
            onSuccess: val => Task.FromResult(successCalled = val == 200),
            onError: _ => Task.FromResult(failureCalled = true),
            onFailure: _ => Task.FromResult(exceptionCalled = true)
        );

        Assert.True(successCalled);
        Assert.False(failureCalled);
        Assert.False(exceptionCalled);
    }

    [Fact]
    public async Task ResolveAsync_WithAllThree_OnlyCallsOnFailureForFailureResult()
    {
        var exception = new Exception("Failure");
        var result = Result.Failure<int>(exception);
        var successCalled = false;
        var errorCalled = false;
        var failureCalled = false;

        await result.ResolveAsync(
            onSuccess: _ => Task.FromResult(successCalled = true),
            onError: err => Task.FromResult(errorCalled = err == exception.Message),
            onFailure: _ => Task.FromResult(failureCalled = true)
        );

        Assert.False(successCalled);
        Assert.False(errorCalled);
        Assert.True(failureCalled);
    }

    [Fact]
    public async Task ResolveAsync_WithAllThree_CallsOnExceptionForFailure()
    {
        var exception = new InvalidOperationException("BOOM!");
        var result = Result.Failure<int>(exception);
        var successCalled = false;
        var failureCalled = false;
        var exceptionCalled = false;

        await result.ResolveAsync(
            onSuccess: _ => Task.FromResult(successCalled = true),
            onError: _ => Task.FromResult(failureCalled = true),
            onFailure: ex => Task.FromResult(exceptionCalled = ex is InvalidOperationException)
        );

        Assert.False(successCalled);
        Assert.False(failureCalled);
        Assert.True(exceptionCalled);
    }

    [Fact]
    public void OnSuccess_Success_ExecutesAction()
    {
        var result = Result.Success(5);
        var executed = false;

        result.OnSuccess(_ => executed = true);

        Assert.True(executed);
    }

    [Fact]
    public void OnSuccess_Failure_DoesNotExecuteAction()
    {
        var result = Result.Failure<int>(new Exception("Failure"));
        var executed = false;

        result.OnSuccess(_ => executed = true);

        Assert.False(executed);
    }

    [Fact]
    public void OnError_Failure_ExecutesAction()
    {
        var result = Result.Failure<int>(new Exception("Failure"));
        var executed = false;

        result.OnError(_ => executed = true);

        Assert.True(executed);
    }

    [Fact]
    public void OnError_Success_DoesNotExecuteAction()
    {
        var result = Result.Success(5);
        var executed = false;

        result.OnError(_ => executed = true);

        Assert.False(executed);
    }

    [Fact]
    public void ValueOrDefault_Failure_ReturnsDefaultValue()
    {
        var result = Result.Failure<int>(new Exception("Failure"));
        var value1 = result.ValueOrDefault();
        var value2 = result.ValueOrDefault(10);

        Assert.Equal(0, value1);
        Assert.Equal(10, value2);
    }

    [Fact]
    public async Task ValueOrDefaultAsync_Success_ReturnsValue()
    {
        var result = Task.FromResult(Result.Success(5));
        var value = await result.ValueOrDefaultAsync(10);

        Assert.Equal(5, value);
    }

    [Fact]
    public async Task ValueOrDefaultAsync_Failure_ReturnsDefaultValue()
    {
        var exception = new Exception("Failure");
        var result = await ValueTask.FromResult(Result.Failure<int>(exception)
        ).ValueOrDefaultAsync(10);
        
        Assert.Equal(10, result);
    }

    [Fact]
    public void SelectMany_Success_ChainsCorrectly()
    {
        var result = Result.Success(5);

        var chainedResult = result.SelectMany(x => Result.Success(x * 2));

        Assert.True(chainedResult);
        Assert.Equal(10, chainedResult.ValueOrDefault());
    }

    [Fact]
    public void SelectMany_Failure_StopsChain()
    {
        var exception = new Exception("Failure");
        var result = Result.Failure<int>(exception);

        var chainedResult = result.SelectMany(x => Result.Success(x * 2));

        Assert.False(chainedResult);
        Assert.Equal(exception.Message, ((Result<int>.Failure)chainedResult).Exception.Message);
    }

    [Fact]
    public async Task SelectManyAsync_Success_ChainsCorrectly()
    {
        var result = Result.Success(5);

        var chainedResult = await result.SelectManyAsync(async x =>
        {
            await Task.Delay(50);
            return Result.Success(x * 2);
        });

        Assert.True(chainedResult);
        Assert.Equal(10, chainedResult.ValueOrDefault());
    }

    [Fact]
    public async Task SelectManyAsync_Failure_StopsChain()
    {
        var exception = new Exception("Failure");
        var result = Result.Failure<int>(exception);

        var chainedResult = await result.SelectManyAsync(async x =>
        {
            await Task.Delay(50);
            return Result.Success(x * 2);
        });

        Assert.False(chainedResult);
        Assert.Equal(exception.Message, ((Result<int>.Failure)chainedResult).Exception.Message);
    }
}
