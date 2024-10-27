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
    public void ImplicitConversion_FromException_ReturnsExceptionalFailureResult()
    {
        var exception = new InvalidOperationException("Something broke");

        Result<int> result = exception;
        Assert.False(result);

        var exceptionalFailure = result as Result<int>.ExceptionalFailure;
        Assert.NotNull(exceptionalFailure);
        Assert.Equal(exception.Message, exceptionalFailure.Exception.Message);
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
    public void Try_WithErrorMessage_ReturnsFailureWithMessage()
    {
        var result = Result.Try(faultyFunc, "Custom Error");

        Assert.False(result);
        Assert.Equal("Custom Error", ((Result<int>.Failure)result).Error);
    }

    [Fact]
    public void OnExceptionalFailure_ExecutesActionOnException()
    {
        var wasCalled = false;

        var result = Result.Failure<int>(new InvalidOperationException("Oops"));

        result.OnExceptionalFailure(ex => wasCalled = ex.Message == "Oops");
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
        var result = Result.Failure<int>("Error");
        var transformedResult = result.Select(x => x * 2);

        Assert.Equal("Error", ((Result<int>.Failure)transformedResult).Error);
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
        var result = Result.Failure<int>("Error");
        var transformedResult = await result.SelectAsync(async x => await Task.FromResult(x * 2));

        Assert.Equal("Error", ((Result<int>.Failure)transformedResult).Error);
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
        var result = Task.FromResult(Result.Failure<int>("Error occurred"));

        var wasCalled = false;
        var chainedResult = await result.SelectAsync(_ => Task.FromResult(wasCalled = true));

        Assert.False(chainedResult);
        Assert.False(wasCalled);
        Assert.Equal("Error occurred", ((Result<bool>.Failure)chainedResult).Error);
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
        var result = Result.Failure<int>("Error occurred");
        var successCalled = false;
        var errorCalled = false;

        result.Resolve(
            onSuccess: _ => successCalled = true,
            onError: err => errorCalled = err == "Error occurred"
        );

        Assert.False(successCalled);
        Assert.True(errorCalled);
    }

    [Fact]
    public void Resolve_WithOnSuccessAndOnError_CallsOnErrorForExceptionalFailure()
    {
        var exception = new InvalidOperationException("Something went wrong");
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
            onFailure: _ => failureCalled = true,
            onException: _ => exceptionCalled = true
        );

        Assert.True(successCalled);
        Assert.False(failureCalled);
        Assert.False(exceptionCalled);
    }

    [Fact]
    public void Resolve_WithAllThree_CallsOnFailureForFailureResult()
    {
        var result = Result.Failure<int>("Failure occurred");
        var successCalled = false;
        var failureCalled = false;
        var exceptionCalled = false;

        result.Resolve(
            onSuccess: _ => successCalled = true,
            onFailure: err => failureCalled = err == "Failure occurred",
            onException: _ => exceptionCalled = true
        );

        Assert.False(successCalled);
        Assert.True(failureCalled);
        Assert.False(exceptionCalled);
    }

    [Fact]
    public void Resolve_WithAllThree_CallsOnExceptionForExceptionalFailure()
    {
        var exception = new InvalidOperationException("Something blew up");
        var result = Result.Failure<int>(exception);
        var successCalled = false;
        var failureCalled = false;
        var exceptionCalled = false;

        result.Resolve(
            onSuccess: _ => successCalled = true,
            onFailure: _ => failureCalled = true,
            onException: ex => exceptionCalled = ex is InvalidOperationException
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
        var result = Result.Failure<int>("Error occurred");
        var successCalled = false;
        var errorCalled = false;

        await result.ResolveAsync(
            onSuccess: _ => Task.FromResult(successCalled = true),
            onError: err => Task.FromResult(errorCalled = err == "Error occurred")
        );

        Assert.False(successCalled);
        Assert.True(errorCalled);
    }

    [Fact]
    public async Task ResolveAsync_WithOnSuccessAndOnError_CallsOnErrorForExceptionalFailure()
    {
        var exception = new InvalidOperationException("Something went wrong");
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
            onFailure: _ => Task.FromResult(failureCalled = true),
            onException: _ => Task.FromResult(exceptionCalled = true)
        );

        Assert.True(successCalled);
        Assert.False(failureCalled);
        Assert.False(exceptionCalled);
    }

    [Fact]
    public async Task ResolveAsync_WithAllThree_CallsOnFailureForFailureResult()
    {
        var result = Result.Failure<int>("Failure occurred");
        var successCalled = false;
        var failureCalled = false;
        var exceptionCalled = false;

        await result.ResolveAsync(
            onSuccess: _ => Task.FromResult(successCalled = true),
            onFailure: err => Task.FromResult(failureCalled = err == "Failure occurred"),
            onException: _ => Task.FromResult(exceptionCalled = true)
        );

        Assert.False(successCalled);
        Assert.True(failureCalled);
        Assert.False(exceptionCalled);
    }

    [Fact]
    public async Task ResolveAsync_WithAllThree_CallsOnExceptionForExceptionalFailure()
    {
        var exception = new InvalidOperationException("Something blew up");
        var result = Result.Failure<int>(exception);
        var successCalled = false;
        var failureCalled = false;
        var exceptionCalled = false;

        await result.ResolveAsync(
            onSuccess: _ => Task.FromResult(successCalled = true),
            onFailure: _ => Task.FromResult(failureCalled = true),
            onException: ex => Task.FromResult(exceptionCalled = ex is InvalidOperationException)
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
        var result = Result.Failure<int>("Error");
        var executed = false;

        result.OnSuccess(_ => executed = true);

        Assert.False(executed);
    }

    [Fact]
    public void OnError_Failure_ExecutesAction()
    {
        var result = Result.Failure<int>("Error");
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
        var result = Result.Failure<int>("Error");
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
        var value = await ValueTask.FromResult(Result.Failure<int>("Error")).ValueOrDefaultAsync(10);;
        Assert.Equal(10, value);
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
        var result = Result.Failure<int>("Initial Error");

        var chainedResult = result.SelectMany(x => Result.Success(x * 2));

        Assert.False(chainedResult);
        Assert.Equal("Initial Error", ((Result<int>.Failure)chainedResult).Error);
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
        var result = Result.Failure<int>("Initial Error");

        var chainedResult = await result.SelectManyAsync(async x =>
        {
            await Task.Delay(50);
            return Result.Success(x * 2);
        });

        Assert.False(chainedResult);
        Assert.Equal("Initial Error", ((Result<int>.Failure)chainedResult).Error);
    }
}