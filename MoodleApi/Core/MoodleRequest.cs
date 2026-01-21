namespace MoodleSdk.Core;

/// <summary>
/// Explicit representation of a Moodle API call.
/// </summary>
public sealed class MoodleRequest
{
    /// <summary>
    /// The name of the Moodle web service function to call.
    /// </summary>
    public string Function { get; init; } = null!;

    /// <summary>
    /// The HTTP method to use (GET or POST).
    /// </summary>
    public HttpMethod Method { get; init; } = HttpMethod.Get;

    /// <summary>
    /// The parameters for the Moodle function.
    /// </summary>
    public IDictionary<string, object> Parameters { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// Optional token override for this specific request.
    /// </summary>
    public string? OverrideToken { get; init; }
}
