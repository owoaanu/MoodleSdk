# Advanced Features

The `MoodleSdk` is designed for extensibility and provides powerful tools for monitoring and customization.

## 1. Hook System

Hooks allow you to intercept every request and response. This is ideal for logging, telemetry, or custom validation.

### Creating a Hook
Implement the `IMoodleClientHook` interface:

```csharp
using MoodleSdk.Hooks;
using MoodleSdk.Core;

public class MyLoggingHook : IMoodleClientHook
{
    public Task OnBeforeRequestAsync(MoodleRequest request)
    {
        Console.WriteLine($"[SDK] Calling {request.Function}...");
        return Task.CompletedTask;
    }

    public Task OnAfterResponseAsync(MoodleRequest request, string rawResponse)
    {
        Console.WriteLine($"[SDK] Received response for {request.Function}");
        return Task.CompletedTask;
    }

    public Task OnErrorAsync(MoodleRequest request, Exception exception)
    {
        Console.WriteLine($"[SDK] Error in {request.Function}: {exception.Message}");
        return Task.CompletedTask;
    }
}
```

### Registering the Hook (DI)
```csharp
builder.Services.AddMoodleSdk(options => { ... })
                .AddHook<MyLoggingHook>();
```

---

## 2. Custom Function Service

Moodle has hundreds of API functions. If a specific function is not yet exposed via a strongly-typed service in this SDK, you can use the `Custom` service to call it.

```csharp
var parameters = new Dictionary<string, object>
{
    { "courseid", 123 },
    { "field", "idnumber" }
};

var result = await _moodle.Custom.ExecuteAsync<MyModel>("mod_assignment_get_assignments", parameters);
```

---

## 3. Error Handling

Instead of checking `IsSuccess` manually everywhere, you can use `ThrowIfFailed()`.

```csharp
var result = await _moodle.Users.GetUsersAsync("username", "john_doe");

// Throws MoodleApiException (or more specific ones like MoodlePermissionException) on error
result.ThrowIfFailed();

var user = result.Data.Users.First();
```

The SDK automatically maps Moodle error codes to specialized exceptions:
- `MoodlePermissionException`: When the token lacks permissions.
- `MoodleSessionException`: When the session has expired or the token is invalid.
- `MoodleValidationException`: When provided parameters are invalid.
