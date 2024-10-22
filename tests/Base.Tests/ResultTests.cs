using RickDotNet.Base;
using RickDotNet.Extensions.Base;

namespace Base.Tests;

public class ResultTests
{
    [Fact]
    public void ResultTry()
    {
        var result = Result.Try(() => 420)
            .OnFailure(Console.WriteLine)
            .ValueOrDefault();
        
        Assert.Equal(420, result);
    }
    
    [Fact]
    public void ResultTryWithException()
    {
        var result = Result.Try(BlowUp)
            .OnFailure(exception => Console.WriteLine(exception))
            .ValueOrDefault();
        
        Assert.Equal(0, result);
    }
    
    private static int BlowUp()
    {
        throw new Exception("oh no");
    }
}