using MoodleSdk.Core;

namespace MoodleSdk.Hooks;

/// <summary>
/// Defines hooks that can be executed during the lifecycle of a Moodle API request.
/// </summary>
public interface IMoodleClientHook
{
    /// <summary>
    /// Executed before a request is sent.
    /// </summary>
    Task OnBeforeRequestAsync(MoodleRequest request);

    /// <summary>
    /// Executed after a response is received, even if it contains a Moodle error.
    /// </summary>
    Task OnAfterResponseAsync(MoodleRequest request, string rawContent);

    /// <summary>
    /// Executed when an exception occurs during the request or processing.
    /// </summary>
    Task OnErrorAsync(MoodleRequest request, Exception exception);
}
