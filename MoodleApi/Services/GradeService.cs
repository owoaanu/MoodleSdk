using MoodleApi.Models;
using MoodleSdk.Core;

namespace MoodleSdk.Services;

/// <summary>
/// Implementation of the IGradeService providing access to Moodle grade functions.
/// </summary>
public sealed class GradeService : IGradeService
{
    private readonly IMoodleClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="GradeService"/> class.
    /// </summary>
    /// <param name="client">The Moodle client.</param>
    public GradeService(IMoodleClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Get grades for a course, optionally filtered by user, component, and activity.
    /// </summary>
    /// <param name="courseId">The ID of the course.</param>
    /// <param name="component">The component name (e.g. 'mod_assign').</param>
    /// <param name="activityId">The activity ID.</param>
    /// <param name="userIds">Optional array of user IDs to filter by.</param>
    /// <returns>The grade category and items.</returns>
    public Task<MoodleResult<Category>> GetGradesAsync(int courseId, string component = "", int activityId = int.MaxValue, string[]? userIds = null)
    {
        var parameters = new Dictionary<string, object>
        {
            { "courseid", courseId }
        };

        if (!string.IsNullOrEmpty(component)) parameters.Add("component", component);
        if (activityId != int.MaxValue) parameters.Add("activityid", activityId);

        if (userIds != null)
        {
            for (int i = 0; i < userIds.Length; i++)
            {
                parameters.Add($"userids[{i}]", userIds[i]);
            }
        }

        return _client.ExecuteAsync<Category>(new MoodleRequest
        {
            Function = MoodleFunctions.Grade.GetGrades,
            Parameters = parameters
        });
    }
}
