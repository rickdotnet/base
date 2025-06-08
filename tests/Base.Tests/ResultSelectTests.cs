using RickDotNet.Base;
using RickDotNet.Extensions.Base;

namespace Base.Tests;

public class ResultSelectTests
{
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
}
