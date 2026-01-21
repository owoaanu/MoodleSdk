using MoodleApi.Models;
using MoodleSdk.Core;

namespace MoodleSdk.Services;

/// <summary>
/// Implementation of the IGroupService providing access to Moodle group functions.
/// </summary>
public sealed class GroupService : IGroupService
{
    private readonly IMoodleClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupService"/> class.
    /// </summary>
    /// <param name="client">The Moodle client.</param>
    public GroupService(IMoodleClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Get groups by their IDs.
    /// </summary>
    /// <param name="groupIds">The IDs of the groups.</param>
    /// <returns>A list of groups.</returns>
    public Task<MoodleResult<Group[]>> GetGroupsAsync(params int[] groupIds)
    {
        var parameters = new Dictionary<string, object>();
        for (int i = 0; i < groupIds.Length; i++)
        {
            parameters.Add($"groupids[{i}]", groupIds[i]);
        }

        return _client.ExecuteAsync<Group[]>(new MoodleRequest
        {
            Function = MoodleFunctions.Group.GetGroups,
            Parameters = parameters
        });
    }

    /// <summary>
    /// Get all groups in a course.
    /// </summary>
    /// <param name="courseId">The ID of the course.</param>
    /// <returns>A list of groups in the course.</returns>
    public Task<MoodleResult<Group[]>> GetCourseGroupsAsync(int courseId)
    {
        return _client.ExecuteAsync<Group[]>(new MoodleRequest
        {
            Function = MoodleFunctions.Group.GetCourseGroups,
            Parameters = new Dictionary<string, object> { { "courseid", courseId } }
        });
    }

    /// <summary>
    /// Create new groups.
    /// </summary>
    /// <param name="groups">A list of group property dictionaries.</param>
    /// <returns>The newly created groups.</returns>
    public Task<MoodleResult<Group[]>> CreateGroupsAsync(IEnumerable<IDictionary<string, object>> groups)
    {
        var parameters = new Dictionary<string, object>();
        int i = 0;
        foreach (var group in groups)
        {
            foreach (var kvp in group)
            {
                parameters.Add($"groups[{i}][{kvp.Key}]", kvp.Value);
            }
            i++;
        }

        return _client.ExecuteAsync<Group[]>(new MoodleRequest
        {
            Function = MoodleFunctions.Group.CreateGroups,
            Method = HttpMethod.Post,
            Parameters = parameters
        });
    }

    /// <summary>
    /// Add members to groups.
    /// </summary>
    /// <param name="members">A list of (GroupId, UserId) pairs to add.</param>
    /// <returns>Success indicator.</returns>
    public Task<MoodleResult<Success>> AddGroupMembersAsync(params (int GroupId, int UserId)[] members)
    {
        var parameters = new Dictionary<string, object>();
        for (int i = 0; i < members.Length; i++)
        {
            parameters.Add($"members[{i}][groupid]", members[i].GroupId);
            parameters.Add($"members[{i}][userid]", members[i].UserId);
        }

        return _client.ExecuteAsync<Success>(new MoodleRequest
        {
            Function = MoodleFunctions.Group.AddGroupMembers,
            Method = HttpMethod.Post,
            Parameters = parameters
        });
    }

    /// <summary>
    /// Delete members from groups.
    /// </summary>
    /// <param name="members">A list of (GroupId, UserId) pairs to delete.</param>
    /// <returns>Success indicator.</returns>
    public Task<MoodleResult<Success>> DeleteGroupMembersAsync(params (int GroupId, int UserId)[] members)
    {
        var parameters = new Dictionary<string, object>();
        for (int i = 0; i < members.Length; i++)
        {
            parameters.Add($"members[{i}][groupid]", members[i].GroupId);
            parameters.Add($"members[{i}][userid]", members[i].UserId);
        }

        return _client.ExecuteAsync<Success>(new MoodleRequest
        {
            Function = MoodleFunctions.Group.DeleteGroupMembers,
            Method = HttpMethod.Post,
            Parameters = parameters
        });
    }
}
