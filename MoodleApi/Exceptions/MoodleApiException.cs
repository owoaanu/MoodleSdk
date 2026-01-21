using MoodleSdk.Core;

namespace MoodleSdk.Exceptions;

/// <summary>
/// Base exception for all Moodle API related errors.
/// </summary>
public class MoodleApiException : Exception
{
    public MoodleError? MoodleError { get; }

    public MoodleApiException(string message, MoodleError? error = null) : base(message)
    {
        MoodleError = error;
    }
}
