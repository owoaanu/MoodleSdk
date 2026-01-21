using Microsoft.Extensions.Logging;
using MoodleSdk.Core;

namespace MoodleSdk.Hooks;

/// <summary>
/// A built-in hook that logs Moodle API interactions.
/// </summary>
public sealed class LoggingHook : IMoodleClientHook
{
    private readonly ILogger<LoggingHook> _logger;

    public LoggingHook(ILogger<LoggingHook> logger)
    {
        _logger = logger;
    }

    public Task OnBeforeRequestAsync(MoodleRequest request)
    {
        _logger.LogInformation("Sending Moodle API request: {Function} ({Method})", request.Function, request.Method);
        return Task.CompletedTask;
    }

    public Task OnAfterResponseAsync(MoodleRequest request, string rawContent)
    {
        _logger.LogDebug("Received Moodle API response for {Function}. Content length: {Length}", request.Function, rawContent.Length);
        return Task.CompletedTask;
    }

    public Task OnErrorAsync(MoodleRequest request, Exception exception)
    {
        _logger.LogError(exception, "Moodle API request failed for {Function}", request.Function);
        return Task.CompletedTask;
    }
}
