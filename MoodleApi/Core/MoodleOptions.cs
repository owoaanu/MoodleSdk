using MoodleApi.Models;

namespace MoodleSdk.Core;

/// <summary>
/// Centralized configuration for the Moodle SDK.
/// </summary>
public sealed class MoodleOptions
{
    /// <summary>
    /// The base URL of the Moodle site.
    /// </summary>
    public Uri BaseUrl { get; set; } = null!;

    /// <summary>
    /// The default token to use for API calls.
    /// </summary>
    public string DefaultToken { get; set; } = null!;

    /// <summary>
    /// The response format (default is JSON).
    /// </summary>
    public MoodleFormat Format { get; set; } = MoodleFormat.JSON;
}
