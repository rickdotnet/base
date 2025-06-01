using RickDotNet.Base;
using RickDotNet.Extensions.Base;

namespace Base.Tests;

public class ResultSelectManyTests
{
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
    
    public class ResultSelectManyAsyncTests
    {
        [Fact]
        public async Task SelectManyAsync_TaskResult_Success_ChainsCorrectly()
        {
            var result = Task.FromResult(Result.Success(5));
            var chainedResult = await result.SelectManyAsync(async x => await Task.FromResult(Result.Success(x * 2)));
            Assert.True(chainedResult);
            Assert.Equal(10, chainedResult.ValueOrDefault());
        }

        [Fact]
        public async Task SelectManyAsync_TaskResult_Failure_StopsChain()
        {
            var exception = new Exception("Failure");
            var result = Task.FromResult(Result.Failure<int>(exception));
            var wasCalled = false;
            var chainedResult = await result.SelectManyAsync<int, int>(async x => { wasCalled = true; return Result.Success(x * 2); });
            Assert.False(chainedResult);
            Assert.False(wasCalled);
            Assert.Equal(exception.Message, ((Result<int>.Failure)chainedResult).Exception.Message);
        }
    }
}

