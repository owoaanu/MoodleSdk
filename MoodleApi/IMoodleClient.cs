using MoodleSdk.Core;
using MoodleSdk.Services;

namespace MoodleSdk;

/// <summary>
/// The primary entry point for the Moodle SDK.
/// </summary>
public interface IMoodleClient : IDisposable
{
    /// <summary>
    /// User management service.
    /// </summary>
    IUserService Users { get; }

    /// <summary>
    /// System and site information service.
    /// </summary>
    ISystemService System { get; }

    /// <summary>
    /// Enrolment and course participation service.
    /// </summary>
    IEnrolmentService Enrolments { get; }

    /// <summary>
    /// Course and category management service.
    /// </summary>
    ICourseService Courses { get; }

    /// <summary>
    /// Group management service.
    /// </summary>
    IGroupService Groups { get; }

    /// <summary>
    /// Grade management service.
    /// </summary>
    IGradeService Grades { get; }

    /// <summary>
    /// Calendar and event service.
    /// </summary>
    ICalendarService Calendar { get; }

    /// <summary>
    /// Custom function service for plugins.
    /// </summary>
    ICustomFunctionService Custom { get; }

    /// <summary>
    /// Executes a Moodle API request.
    /// </summary>
    /// <typeparam name="T">The type of the expected data.</typeparam>
    /// <param name="request">The request configuration.</param>
    /// <returns>A MoodleResult containing the response data or error information.</returns>
    Task<MoodleResult<T>> ExecuteAsync<T>(MoodleRequest request);
}
