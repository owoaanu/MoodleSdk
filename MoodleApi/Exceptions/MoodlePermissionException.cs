using MoodleSdk.Core;

namespace MoodleSdk.Exceptions;

/// <summary>
/// Exception thrown when a user does not have permission to perform an action in Moodle.
/// </summary>
public class MoodlePermissionException : MoodleApiException
{
    public MoodlePermissionException(string message, MoodleError? error = null) : base(message, error)
    {
    }
}
