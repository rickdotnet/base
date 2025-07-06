using RickDotNet.Base;
using RickDotNet.Extensions.Base;

namespace Base.Tests;

public class ResultOrTests
{
    [Fact]
    public void Or_Success_ReturnsOriginalResult()
    {
        var result = Result.Success(42);
        var called = false;
        var orResult = result.Or(_ => { called = true; return -1; });
        Assert.True(orResult);
        Assert.Equal(42, orResult.ValueOrDefault());
        Assert.False(called);
    }

    [Fact]
    public void Or_Error_InvokesOnErrorAndReturnsResult()
    {
        var result = Result.Error<int>("error");
        var orResult = result.Or(msg => msg.Length);
        
        Assert.False(result);
        Assert.True(orResult);
        Assert.Equal(5, orResult.ValueOrDefault());
    }

    [Fact]
    public void Or_Failure_InvokesOnErrorAndReturnsResult()
    {
        var ex = new Exception("fail");
        var result = Result.Failure<int>(ex);
        var orResult = result.Or(msg => msg.Length);
        
        Assert.False(result);
        Assert.True(orResult);
        Assert.Equal(4, orResult.ValueOrDefault());
    }

    [Fact]
    public async Task OrAsync_Success_ReturnsOriginalResult()
    {
        var result = Result.Success(100);
        var called = false;
        var orResult = await result.OrAsync(_ => { called = true; return -1; });
        
        Assert.True(orResult);
        Assert.Equal(100, orResult.ValueOrDefault());
        Assert.False(called);
    }

    [Fact]
    public async Task OrAsync_Error_InvokesOnErrorAndReturnsResult()
    {
        var result = Result.Error<int>("bad");
        var orResult = await result.OrAsync(msg => msg.Length);
        Assert.True(orResult);
        Assert.Equal(3, orResult.ValueOrDefault());
    }

    [Fact]
    public async Task OrAsync_Failure_InvokesOnErrorAndReturnsResult()
    {
        var ex = new Exception("fail");
        var result = Result.Failure<int>(ex);
        var orResult = await result.OrAsync(msg => msg.Length);
        Assert.True(orResult);
        Assert.Equal(4, orResult.ValueOrDefault());
    }

    [Fact]
    public async Task OrAsync_TaskResult_Error_InvokesOnErrorAndReturnsResult()
    {
        var result = Task.FromResult(Result.Error<int>("err"));
        var orResult = await result.OrAsync(msg => msg.Length);
        Assert.True(orResult);
        Assert.Equal(3, orResult.ValueOrDefault());
    }

    [Fact]
    public async Task OrAsync_TaskResult_Failure_InvokesOnErrorAndReturnsResult()
    {
        var ex = new Exception("fail");
        var result = Task.FromResult(Result.Failure<int>(ex));
        var orResult = await result.OrAsync(msg => msg.Length);
        Assert.True(orResult);
        Assert.Equal(4, orResult.ValueOrDefault());
    }

    [Fact]
    public async Task OrAsync_TaskResult_Success_ReturnsOriginalResult()
    {
        var result = Task.FromResult(Result.Success(7));
        var called = false;
        var orResult = await result.OrAsync(_ => { called = true; return -1; });
        Assert.True(orResult);
        Assert.Equal(7, orResult.ValueOrDefault());
        Assert.False(called);
    }
}

