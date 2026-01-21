using MoodleApi.Models;
using MoodleSdk.Core;

namespace MoodleSdk.Services;

/// <summary>
/// Provides access to enrolment and course participation functions in the Moodle API.
/// </summary>
public interface IEnrolmentService
{
    /// <summary>
    /// Gets the list of courses where a user is enrolled.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A MoodleResult containing an array of courses.</returns>
    Task<MoodleResult<Cources[]>> GetUserCoursesAsync(int userId);
    Task<MoodleResult<Success>> AssignRoleAsync(int roleId, int userId, string contextId = "", string contextLevel = "", int? instanceId = null);
    Task<MoodleResult<Success>> UnassignRoleAsync(int roleId, int userId, string contextId = "", string contextLevel = "", int? instanceId = null);

    /// <summary>
    /// Enrols a user in a course with a specific role.
    /// </summary>
    /// <param name="roleId">The role ID for the enrolment (e.g., student = 5, teacher = 3).</param>
    /// <param name="userId">The ID of the user to enrol.</param>
    /// <param name="courseId">The ID of the course.</param>
    /// <param name="timeStart">Enrolment start time (Unix timestamp).</param>
    /// <param name="timeEnd">Enrolment end time (Unix timestamp).</param>
    /// <param name="suspend">Whether the enrolment is suspended.</param>
    /// <returns>A MoodleResult indicating success or failure.</returns>
    Task<MoodleResult<Success>> EnrolUserAsync(int roleId, int userId, int courseId, int? timeStart = null, int? timeEnd = null, int? suspend = null);
    Task<MoodleResult<Success>> EnrolUserAsync(int roleId, int userId, List<int> courseIds, int? timeStart = null, int? timeEnd = null, int? suspend = null);
    Task<MoodleResult<Success>> EnrolMultipleUsersAsync(int roleId, List<int> userIds, List<int> courseIds, int? timeStart = null, int? timeEnd = null, int? suspend = null);
}
