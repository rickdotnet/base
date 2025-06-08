using RickDotNet.Base;
using RickDotNet.Extensions.Base;

namespace Base.Tests;

public class ResultTryTests
{
    private readonly Func<Result<int>> faultyFunc = () => throw new InvalidOperationException();

    [Fact]
    public void Try_WithErrorMessage_ReturnsErrorWithMessage()
    {
        var result = Result.Try(faultyFunc, "Error");
        Assert.False(result);
        Assert.Equal("Error", ((Result<int>.Error)result).ErrorMessage);
    }

    [Fact]
    public void Try_Action_WithAndWithoutErrorMessage()
    {
        // Success case
        var result1 = Result.Try(() => "no-op");
        Assert.True(result1);

        // Error case
        var result2 = Result.Try(() => throw new Exception("fail"));
        Assert.False(result2);
        Assert.Equal("fail", ((Result<Unit>.Failure)result2).Exception.Message);

        // Error message overload
        var result3 = Result.Try(() => throw new Exception(), "error");
        Assert.False(result3);
        Assert.Equal("error", ((Result<Unit>.Error)result3).ErrorMessage);
    }

    [Fact]
    public void Try_FuncResultT_WithAndWithoutErrorMessage()
    {
        // Success case
        var result1 = Result.Try(() => Result.Success(42));
        Assert.True(result1);
        Assert.Equal(42, ((Result<int>.Success)result1).Value);

        // Exception case
        var result2 = Result.Try(() => throw new Exception("fail"));
        Assert.False(result2);
        Assert.Equal("fail", ((Result<Unit>.Failure)result2).Exception.Message);

        // Error message overload
        var result3 = Result.Try(() => throw new Exception(), "error");
        Assert.False(result3);
        Assert.Equal("error", ((Result<Unit>.Error)result3).ErrorMessage);
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
        var result1 = await Result.TryAsync(() => Task.FromResult(5));
        Assert.True(result1);
        Assert.Equal(5, result1.ValueOrDefault());
        var result2 = await Result.TryAsync(() => Task.CompletedTask);
        Assert.True(result2);
        var result3 = await Result.TryAsync(() => Task.FromException<int>(new Exception("Oops")));
        Assert.False(result3);
        result3.OnError(error => Assert.Equal("Oops", error));
    }

    [Fact]
    public async Task TryAsync_Action_WithAndWithoutErrorMessage()
    {
        // Success case
        var result1 = await Result.TryAsync(() => Task.CompletedTask);
        Assert.True(result1);

        // Exception case
        var result2 = await Result.TryAsync(() => throw new Exception("fail"));
        Assert.False(result2);
        Assert.Equal("fail", ((Result<Unit>.Failure)result2).Exception.Message);

        // Error message overload
        var result3 = await Result.TryAsync(() => throw new Exception(), "error");
        Assert.False(result3);
        Assert.Equal("error", ((Result<Unit>.Error)result3).ErrorMessage);
    }

    [Fact]
    public async Task TryAsync_FuncResultT_WithAndWithoutErrorMessage()
    {
        // Success case
        var result1 = await Result.TryAsync(() => Task.FromResult(Result.Success(42)));
        Assert.True(result1);
        Assert.Equal(42, ((Result<int>.Success)result1).Value);

        // Exception case
        var result2 = await Result.TryAsync<int>(() => throw new Exception("fail"));
        Assert.False(result2);
        Assert.Equal("fail", ((Result<int>.Failure)result2).Exception.Message);

        // Error message overload
        var result3 = await Result.TryAsync<int>(() => throw new Exception(), "error");
        Assert.False(result3);
        Assert.Equal("error", ((Result<int>.Error)result3).ErrorMessage);
    }
}
