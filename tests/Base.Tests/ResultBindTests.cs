using RickDotNet.Base;
using RickDotNet.Extensions.Base;

namespace Base.Tests;

public class ResultBindTests
{
    [Fact]
    public void Bind_Success_ChainsCorrectly()
    {
        var result = Result.Success(5);
        var chainedResult = result.Bind(x => Result.Success(x * 2));
        Assert.True(chainedResult);
        Assert.Equal(10, chainedResult.ValueOrDefault());
    }

    [Fact]
    public void Bind_Failure_StopsChain()
    {
        var exception = new Exception("Failure");
        var result = Result.Failure<int>(exception);
        var chainedResult = result.Bind(x => Result.Success(x * 2));
        Assert.False(chainedResult);
        Assert.Equal(exception.Message, ((Result<int>.Failure)chainedResult).Exception.Message);
    }

    [Fact]
    public async Task BindAsync_Success_ChainsCorrectly()
    {
        var result = Result.Success(5);
        var chainedResult = await result.BindAsync(async x =>
        {
            await Task.Delay(50);
            return Result.Success(x * 2);
        });
        Assert.True(chainedResult);
        Assert.Equal(10, chainedResult.ValueOrDefault());
    }

    [Fact]
    public async Task BindAsync_Failure_StopsChain()
    {
        var exception = new Exception("Failure");
        var result = Result.Failure<int>(exception);
        var chainedResult = await result.BindAsync(async x =>
        {
            await Task.Delay(50);
            return Result.Success(x * 2);
        });
        Assert.False(chainedResult);
        Assert.Equal(exception.Message, ((Result<int>.Failure)chainedResult).Exception.Message);
    }

    public class ResultBindAsyncTests
    {
        [Fact]
        public async Task BindAsync_TaskResult_Success_ChainsCorrectly()
        {
            var result = Task.FromResult(Result.Success(5));
            var chainedResult = await result.BindAsync(async x => await Task.FromResult(Result.Success(x * 2)));
            Assert.True(chainedResult);
            Assert.Equal(10, chainedResult.ValueOrDefault());
        }

        [Fact]
        public async Task BindAsync_TaskResult_Failure_StopsChain()
        {
            var exception = new Exception("Failure");
            var result = Task.FromResult(Result.Failure<int>(exception));
            var wasCalled = false;
            var chainedResult = await result.BindAsync(x =>
            {
                wasCalled = true;
                return Task.FromResult(Result.Success(x * 2));
            });
            Assert.False(chainedResult);
            Assert.False(wasCalled);
            Assert.Equal(exception.Message, ((Result<int>.Failure)chainedResult).Exception.Message);
        }
    }
}
