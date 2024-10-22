using RickDotNet.Base;
using RickDotNet.Extensions.Base;

namespace Base.Tests;

public class ResultTests
{
    [Fact]
    public void ResultTry()
    {
        var result = Result.Try(() => 420)
            .OnExceptionalFailure(Console.WriteLine)
            .ValueOrDefault();

        Assert.Equal(420, result);
    }

    [Fact]
    public void ResultTryWithException()
    {
        var result = Result.Try(BlowUp)
            .OnExceptionalFailure(exception => Console.WriteLine(exception))
            .ValueOrDefault();

        Assert.Equal(0, result);
    }

    private static int BlowUp()
    {
        throw new Exception("oh no");
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
        var value = result.ValueOrDefault(10);

        Assert.Equal(10, value);
    }
}