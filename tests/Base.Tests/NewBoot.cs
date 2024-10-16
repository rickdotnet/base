using RickDotNet.Extensions.Base;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace RickDotNet.Base.Tests;

public class NewBoot
{
    public static async Task Goofin()
    {
        var result = Result.Try(() => 420);
        var result2 = await Result.Try(async () => await Task.FromResult(69));
        result2!.Resolve(
            onSuccess: success => Console.WriteLine(success),
            onFailure: Console.WriteLine,
            onException: Console.WriteLine
        );
        
        var result3 = await result2.SelectMany<int, int>(async x => 5);
        var result4 = await result2.SelectMany(async x => Result.Success(5));
    }
}


#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously