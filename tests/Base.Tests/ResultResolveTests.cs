using RickDotNet.Base;
using RickDotNet.Extensions.Base;

namespace Base.Tests;

public class ResultResolveTests
{
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

    // ...other async Resolve tests can be added here as needed...
}

