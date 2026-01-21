using MoodleSdk.Core;

namespace MoodleSdk.Exceptions;

/// <summary>
/// Exception thrown when a Moodle API request fails validation (e.g., invalid parameters).
/// </summary>
public class MoodleValidationException : MoodleApiException
{
    public MoodleValidationException(string message, MoodleError? error = null) : base(message, error)
    {
    }
}
