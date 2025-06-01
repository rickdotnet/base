using RickDotNet.Base;
using RickDotNet.Extensions.Base;

namespace Base.Tests;

public class ResultTryTests
{
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
}

