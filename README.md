# RickDotNet.Base

A quiet spot for things that do stuff. Ready to copy, paste, and slap into your codebase.

![Ain't Much](/honest.gif)

## Result

A poor man's Result type. See [Documentation](docs/Result.md).

```csharp
var result = Result.Try(() => int.Parse("not a number"));
if (result.Successful)
{
    // do your thing
    var thing = result.ValueOrDefault();
}

// do other things
int? other = result.ValueOrDefault();

// or get funcy
result.Resolve(
    onSuccess: value => Console.WriteLine($"Value: {value}"),
    onError: error => Console.WriteLine($"Error: {error}")
);
```
