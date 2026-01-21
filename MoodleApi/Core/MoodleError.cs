namespace MoodleSdk.Core;

/// <summary>
/// Represents an error returned by the Moodle API.
/// </summary>
public sealed class MoodleError
{
    public string? Exception { get; init; }
    public string? ErrorCode { get; init; }
    public string? Message { get; init; }
    public string? DebugInfo { get; init; }
}
