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

    [Fact]
    public void OnFailure_ExecutesActionOnException()
    {
        var wasCalled = false;

        var result = Result.Failure<int>(new InvalidOperationException("Oops"));

        result.OnFailure(ex => wasCalled = ex.Message == "Oops");
        Assert.True(wasCalled);
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
}
