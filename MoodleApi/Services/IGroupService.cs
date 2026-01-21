using MoodleApi.Models;
using MoodleSdk.Core;

namespace MoodleSdk.Services;

/// <summary>
/// Provides access to group management functions in the Moodle API.
/// </summary>
public interface IGroupService
{
    Task<MoodleResult<Group[]>> GetGroupsAsync(params int[] groupIds);
    Task<MoodleResult<Group[]>> GetCourseGroupsAsync(int courseId);
    Task<MoodleResult<Group[]>> CreateGroupsAsync(IEnumerable<IDictionary<string, object>> groups);
    Task<MoodleResult<Success>> AddGroupMembersAsync(params (int GroupId, int UserId)[] members);
    Task<MoodleResult<Success>> DeleteGroupMembersAsync(params (int GroupId, int UserId)[] members);
}
