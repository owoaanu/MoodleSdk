namespace MoodleSdk.Core;

/// <summary>
/// Represents a warning returned by the Moodle API.
/// </summary>
public sealed class MoodleWarning
{
    public int ItemId { get; init; }
    public string? Item { get; init; }
    public string? WarningCode { get; init; }
    public string? Message { get; init; }
}
