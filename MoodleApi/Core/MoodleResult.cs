namespace MoodleSdk.Core;

using MoodleSdk.Exceptions;

/// <summary>
/// Represents the result of a Moodle API call.
/// </summary>
/// <typeparam name="T">The type of the data returned.</typeparam>
public sealed class MoodleResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public MoodleError? Error { get; init; }
    public IReadOnlyList<MoodleWarning> Warnings { get; init; } = Array.Empty<MoodleWarning>();

    /// <summary>
    /// Throws a specialized MoodleApiException if the result is not successful.
    /// </summary>
    public void ThrowIfFailed()
    {
        if (IsSuccess) return;

        var message = Error?.Message ?? "Moodle API call failed";
        var errorCode = Error?.ErrorCode;

        throw errorCode switch
        {
            "nopermissions" => new MoodlePermissionException(message, Error),
            "invalidtoken" or "invalidlogin" or "sessionexpired" => new MoodleSessionException(message, Error),
            "invalidparameter" or "missingparam" => new MoodleValidationException(message, Error),
            _ => new MoodleApiException(message, Error)
        };
    }

    public static MoodleResult<T> Success(T data, IReadOnlyList<MoodleWarning>? warnings = null) => new()
    {
        IsSuccess = true,
        Data = data,
        Warnings = warnings ?? Array.Empty<MoodleWarning>()
    };

    public static MoodleResult<T> Failure(MoodleError error, IReadOnlyList<MoodleWarning>? warnings = null) => new()
    {
        IsSuccess = false,
        Error = error,
        Warnings = warnings ?? Array.Empty<MoodleWarning>()
    };
}
