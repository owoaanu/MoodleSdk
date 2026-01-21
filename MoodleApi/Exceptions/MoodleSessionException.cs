using MoodleSdk.Core;

namespace MoodleSdk.Exceptions;

/// <summary>
/// Exception thrown when a Moodle session is invalid or has expired.
/// </summary>
public class MoodleSessionException : MoodleApiException
{
    public MoodleSessionException(string message, MoodleError? error = null) : base(message, error)
    {
    }
}
