# Result<T>

A simple way to handle success and failure without throwing exceptions around.

## Basics

Three states: Success, Error, or Failure.

```csharp
// Success with a value
var success = Result.Success(42);

// Error with a message
var error = Result.Error<int>("Something went wrong");

// Failure with an exception
var failure = Result.Failure<int>(new Exception("Oops"));
```

## Try Things

For the bad boy operations:

```csharp
var result = Result.Try(() => int.Parse("not a number"));
// Returns Result<int> instead of throwing

var asyncResult = await Result.TryAsync(async () => await SomeAsyncOperation());
```

## Dirty Access

*Purists hate us, 'cause they ain't us.* - Knife Hands McGraw

```csharp
var result = Result.Try(() => int.Parse("not a number"));
if (result.Successful)
{
    // do your thing
    var thing = result.ValueOrDefault();
}

// do other things
int? other = result.ValueOrDefault();

result.Resolve(
    onSuccess: value => Console.WriteLine($"Value: {value}"),
    onError: error => Console.WriteLine($"Error: {error}")
);
```

## Chain Operations

```csharp
var result = await Task.FromResult(Result.Success(5));
result
    .Select(x => x * 2)
    .Bind(SomeOperation<int>)
    .Resolve(
        success => Console.WriteLine(success),
        error => Console.WriteLine(error),
        failure => Console.WriteLine(failure.Message)
    );

Result<TResult> SomeOperation<TResult>(int i)
{
    throw new NotImplementedException();
}
```

## Async Support

There are async versions of most methods. The unit tests are the best place to see examples.

Example: `TryAsync` with `Task<Result<T>>` for async operations:

```csharp
var value = await Result
        .TryAsync(async () => await SomeAsyncOperation())
        .ValueOrDefaultAsync();

Task<int> SomeAsyncOperation()
{
    return Task.FromResult(5);
}
```

See the unit tests for more examples.
