using MoodleApi.Models;
using MoodleSdk.Core;

namespace MoodleSdk.Services;

/// <summary>
/// Provides access to grade management functions in the Moodle API.
/// </summary>
public interface IGradeService
{
    Task<MoodleResult<Category>> GetGradesAsync(int courseId, string component = "", int activityId = int.MaxValue, string[]? userIds = null);
}
