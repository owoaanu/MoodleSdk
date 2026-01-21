using MoodleApi.Models;
using MoodleSdk.Core;

namespace MoodleSdk.Services;

/// <summary>
/// Implementation of the IEnrolmentService providing access to Moodle enrolment and role functions.
/// </summary>
public sealed class EnrolmentService : IEnrolmentService
{
    private readonly IMoodleClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnrolmentService"/> class.
    /// </summary>
    /// <param name="client">The Moodle client.</param>
    public EnrolmentService(IMoodleClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Get the list of courses a user is enrolled in.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of courses.</returns>
    public Task<MoodleResult<Cources[]>> GetUserCoursesAsync(int userId)
    {
        return _client.ExecuteAsync<Cources[]>(new MoodleRequest
        {
            Function = MoodleFunctions.Enrolment.GetUsersCourses,
            Parameters = new Dictionary<string, object>
            {
                { "userid", userId }
            }
        });
    }

    /// <summary>
    /// Assign a role to a user in a specific context.
    /// </summary>
    /// <param name="roleId">The ID of the role to assign.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="contextId">The context ID.</param>
    /// <param name="contextLevel">The context level.</param>
    /// <param name="instanceId">The instance ID.</param>
    /// <returns>Success indicator.</returns>
    public Task<MoodleResult<Success>> AssignRoleAsync(int roleId, int userId, string contextId = "", string contextLevel = "", int? instanceId = null)
    {
        var parameters = new Dictionary<string, object>
        {
            { "assignments[0][roleid]", roleId },
            { "assignments[0][userid]", userId }
        };
        if (!string.IsNullOrEmpty(contextId)) parameters.Add("assignments[0][contextid]", contextId);
        if (!string.IsNullOrEmpty(contextLevel)) parameters.Add("assignments[0][contextlevel]", contextLevel);
        if (instanceId.HasValue) parameters.Add("assignments[0][instanceid]", instanceId.Value);

        return _client.ExecuteAsync<Success>(new MoodleRequest
        {
            Function = MoodleFunctions.Role.AssignRoles,
            Method = HttpMethod.Post,
            Parameters = parameters
        });
    }

    /// <summary>
    /// Unassign a role from a user in a specific context.
    /// </summary>
    /// <param name="roleId">The ID of the role to unassign.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="contextId">The context ID.</param>
    /// <param name="contextLevel">The context level.</param>
    /// <param name="instanceId">The instance ID.</param>
    /// <returns>Success indicator.</returns>
    public Task<MoodleResult<Success>> UnassignRoleAsync(int roleId, int userId, string contextId = "", string contextLevel = "", int? instanceId = null)
    {
        var parameters = new Dictionary<string, object>
        {
            { "unassignments[0][roleid]", roleId },
            { "unassignments[0][userid]", userId }
        };
        if (!string.IsNullOrEmpty(contextId)) parameters.Add("unassignments[0][contextid]", contextId);
        if (!string.IsNullOrEmpty(contextLevel)) parameters.Add("unassignments[0][contextlevel]", contextLevel);
        if (instanceId.HasValue) parameters.Add("unassignments[0][instanceid]", instanceId.Value);

        return _client.ExecuteAsync<Success>(new MoodleRequest
        {
            Function = MoodleFunctions.Role.UnassignRoles,
            Method = HttpMethod.Post,
            Parameters = parameters
        });
    }

    /// <summary>
    /// Enrols a user in a course with a specific role.
    /// </summary>
    /// <param name="roleId">The role ID for the enrolment (e.g., student = 5, teacher = 3).</param>
    /// <param name="userId">The ID of the user to enrol.</param>
    /// <param name="courseId">The ID of the course.</param>
    /// <param name="timeStart">Enrolment start time (Unix timestamp).</param>
    /// <param name="timeEnd">Enrolment end time (Unix timestamp).</param>
    /// <param name="suspend">Whether the enrolment is suspended (1 for suspended, 0 for active).</param>
    /// <returns>A MoodleResult indicating success or failure.</returns>
    public Task<MoodleResult<Success>> EnrolUserAsync(int roleId, int userId, int courseId, int? timeStart = null, int? timeEnd = null, int? suspend = null)
    {
        var parameters = new Dictionary<string, object>
        {
            { "enrolments[0][roleid]", roleId },
            { "enrolments[0][userid]", userId },
            { "enrolments[0][courseid]", courseId }
        };

        if (timeStart.HasValue) parameters.Add("enrolments[0][timestart]", timeStart.Value);
        if (timeEnd.HasValue) parameters.Add("enrolments[0][timeend]", timeEnd.Value);
        if (suspend.HasValue) parameters.Add("enrolments[0][suspend]", suspend.Value);

        return _client.ExecuteAsync<Success>(new MoodleRequest
        {
            Function = MoodleFunctions.Enrolment.ManualEnrolUsers,
            Method = HttpMethod.Post,
            Parameters = parameters
        });
    }

    /// <summary>
    /// Enrols a user in the specified courses with a specific role.
    /// </summary>
    /// <param name="roleId">The role ID for the enrolment (e.g., student = 5, teacher = 3).</param>
    /// <param name="userId">The ID of the user to enrol.</param>
    /// <param name="courseIds">The list of course IDs to enrol the user in.</param>
    /// <param name="timeStart">Enrolment start time (Unix timestamp).</param>
    /// <param name="timeEnd">Enrolment end time (Unix timestamp).</param>
    /// <param name="suspend">Whether the enrolment is suspended (1 for suspended, 0 for active).</param>
    /// <returns>A MoodleResult indicating success or failure.</returns>
    public Task<MoodleResult<Success>> EnrolUserAsync(int roleId, int userId, List<int> courseIds, int? timeStart = null, int? timeEnd = null, int? suspend = null)
    {
        if(courseIds.Count == 0) throw new ArgumentException("At least one course must be specified.");

        var parameters = new Dictionary<string, object>();
        for(int i = 0; i < courseIds.Count; i++)
        {
            parameters.Add("enrolments[" + i + "][roleid]", roleId);
            parameters.Add("enrolments[" + i + "][userid]", userId);
            parameters.Add("enrolments[" + i + "][courseid]", courseIds[i]);

            if (timeStart.HasValue) parameters.Add("enrolments[" + i + "][timestart]", timeStart.Value);
            if (timeEnd.HasValue) parameters.Add("enrolments[" + i + "][timeend]", timeEnd.Value);
            if (suspend.HasValue) parameters.Add("enrolments[" + i + "][suspend]", suspend.Value);
        }

        return _client.ExecuteAsync<Success>(new MoodleRequest
        {
            Function = MoodleFunctions.Enrolment.ManualEnrolUsers,
            Method = HttpMethod.Post,
            Parameters = parameters
        });
    }

    /// <summary>
    /// Enrols a user in the specified courses with a specific role.
    /// </summary>
    /// <param name="roleId">The role ID for the enrolment (e.g., student = 5, teacher = 3).</param>
    /// <param name="userIds">The IDs of the users to enrol.</param>
    /// <param name="courseIds">The list of course IDs to enrol the users in.</param>
    /// <param name="timeStart">Enrolment start time (Unix timestamp).</param>
    /// <param name="timeEnd">Enrolment end time (Unix timestamp).</param>
    /// <param name="suspend">Whether the enrolment is suspended (1 for suspended, 0 for active).</param>
    /// <returns>A MoodleResult indicating success or failure.</returns>
    public async Task<MoodleResult<Success>> EnrolMultipleUsersAsync(int roleId, List<int> userIds, List<int> courseIds, int? timeStart = null, int? timeEnd = null, int? suspend = null)
    {
        if (courseIds.Count == 0 || userIds.Count == 0) 
            throw new ArgumentException("At least one user and one course must be specified.");

        // 1. Flatten the requirements (User x Course)
        var allEnrolments = from user in userIds
                            from course in courseIds
                            select new { UserId = user, CourseId = course };

        // 2. Define a batch size (Moodle usually handles 100-200 well per request)
        const int batchSize = 200;
        var batches = allEnrolments.Chunk(batchSize);

        MoodleResult<Success> finalResult = new();

        foreach (var batch in batches)
        {
            var parameters = new Dictionary<string, object>();
            int i = 0;

            foreach (var item in batch)
            {
                string baseKey = $"enrolments[{i}]";
                parameters.Add($"{baseKey}[roleid]", roleId);
                parameters.Add($"{baseKey}[userid]", item.UserId);
                parameters.Add($"{baseKey}[courseid]", item.CourseId);

                if (timeStart.HasValue) parameters.Add($"{baseKey}[timestart]", timeStart.Value);
                if (timeEnd.HasValue) parameters.Add($"{baseKey}[timeend]", timeEnd.Value);
                if (suspend.HasValue) parameters.Add($"{baseKey}[suspend]", suspend.Value);
                i++;
            }

            // 3. Execute the batch
            finalResult = await _client.ExecuteAsync<Success>(new MoodleRequest
            {
                Function = MoodleFunctions.Enrolment.ManualEnrolUsers,
                Method = HttpMethod.Post,
                Parameters = parameters
            });

            // Optional: If one batch fails, you might want to stop or log it
            // if (!finalResult.IsSuccess) return finalResult; 
        }

        return finalResult;
    }
}
