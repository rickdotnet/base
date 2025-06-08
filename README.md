# RickDotNet.Base

A quiet spot for things that do stuff. Ready to copy, paste, and slap into your codebase.

![Ain't Much](/honest.gif)

## Result

A poor man's Result type. See [Documentation](docs/Result.md).

```csharp
var result = Result.Try(() => int.Parse("not a number"));
//var result = Result.Success(42);

// for the nasty ones
if (result)
{
    // get down to business
    var business = result.ValueOrDefault();
}

// for the annoying ones
if (result.Successful)
{
    // do your thing
    int thing = result.ValueOrDefault();
}

// for the lazy ones
int? other = result.ValueOrDefault();

// for the funcy ones
result.Resolve(
    onSuccess: value => Console.WriteLine($"Value: {value}"),
    onError: error => Console.WriteLine($"Error: {error}")
);
```
