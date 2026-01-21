using MoodleApi.Models;
using MoodleSdk.Core;

namespace MoodleSdk.Services;

/// <summary>
/// Implementation of the ICourseService providing access to Moodle course and category functions.
/// </summary>
public sealed class CourseService : ICourseService
{
    private readonly IMoodleClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="CourseService"/> class.
    /// </summary>
    /// <param name="client">The Moodle client.</param>
    public CourseService(IMoodleClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Get course categories based on criteria.
    /// </summary>
    /// <param name="criteriaKey">The key to search by (e.g. 'name', 'id').</param>
    /// <param name="criteriaValue">The value to match.</param>
    /// <param name="addSubCategories">Whether to include subcategories (1 for yes, 0 for no).</param>
    /// <returns>A list of categories.</returns>
    public Task<MoodleResult<Category[]>> GetCategoriesAsync(string criteriaKey, string criteriaValue, int addSubCategories = 1)
    {
        var parameters = new Dictionary<string, object>
        {
            { "criteria[0][key]", criteriaKey },
            { "criteria[0][value]", criteriaValue },
            { "addsubcategories", addSubCategories }
        };

        return _client.ExecuteAsync<Category[]>(new MoodleRequest
        {
            Function = MoodleFunctions.Course.GetCategories,
            Parameters = parameters
        });
    }

    /// <summary>
    /// Get a list of courses.
    /// </summary>
    /// <param name="options">Optional course ID to filter by.</param>
    /// <returns>A list of courses.</returns>
    public Task<MoodleResult<Course[]>> GetCoursesAsync(int[]? options)
    {
        var parameters = new Dictionary<string, object>();
        if (options != null)
        {
            for (int i = 0; i < options.Length; i++)
            {
                parameters.Add($"options[ids][{i}]", options[i]);
            }
        }

        return _client.ExecuteAsync<Course[]>(new MoodleRequest
        {
            Function = MoodleFunctions.Course.GetCourses,
            Parameters = parameters
        });
    }

    /// <summary>
    /// Get a list of courses.
    /// </summary>
    /// <param name="options">Optional course ID to filter by.</param>
    /// <returns>A list of courses.</returns>
    public Task<MoodleResult<MoodleCourseListResponse>> GetCoursesByFieldAsync(string field, string value)
    {
        var parameters = new Dictionary<string, object>
        {
            { "field", field },
            { "value", value }
        };

        return _client.ExecuteAsync<MoodleCourseListResponse>(new MoodleRequest
        {
            Function = MoodleFunctions.Course.GetCoursesByField,
            Parameters = parameters
        });
    }

    /// <summary>
    /// Get the contents (sections, modules) of a course.
    /// </summary>
    /// <param name="courseId">The ID of the course.</param>
    /// <returns>A list of course contents.</returns>
    public Task<MoodleResult<Content[]>> GetContentsAsync(int courseId)
    {
        return _client.ExecuteAsync<Content[]>(new MoodleRequest
        {
            Function = MoodleFunctions.Course.GetContents,
            Parameters = new Dictionary<string, object> { { "courseid", courseId } }
        });
    }

    /// <summary>
    /// Get users enrolled in a specific course.
    /// </summary>
    /// <param name="courseId">The ID of the course.</param>
    /// <returns>A list of enrolled users.</returns>
    public Task<MoodleResult<EnrolledUser[]>> GetEnrolledUsersByCourseAsync(int courseId)
    {
        return _client.ExecuteAsync<EnrolledUser[]>(new MoodleRequest
        {
            Function = MoodleFunctions.Enrolment.GetEnrolledUsers,
            Parameters = new Dictionary<string, object> { { "courseid", courseId } }
        });
    }

    /// <summary>
    /// Create one or more new courses.
    /// </summary>
    /// <param name="courses">The course details to create.</param>
    /// <returns>The newly created course information.</returns>
    public Task<MoodleResult<NewCourse[]>> CreateCoursesAsync(params MoodleSdk.Models.CreateCourseRequest[] courses)
    {
        var parameters = new Dictionary<string, object>();
        for (int i = 0; i < courses.Length; i++)
        {
            var course = courses[i];
            parameters.Add($"courses[{i}][fullname]", course.FullName);
            parameters.Add($"courses[{i}][shortname]", course.ShortName);
            parameters.Add($"courses[{i}][categoryid]", course.CategoryId);
            
            if (!string.IsNullOrEmpty(course.IdNumber)) parameters.Add($"courses[{i}][idnumber]", course.IdNumber);
            if (!string.IsNullOrEmpty(course.Summary)) parameters.Add($"courses[{i}][summary]", course.Summary);
            if (course.SummaryFormat.HasValue) parameters.Add($"courses[{i}][summaryformat]", course.SummaryFormat.Value);
            if (!string.IsNullOrEmpty(course.Format)) parameters.Add($"courses[{i}][format]", course.Format);
            if (course.ShowGrades.HasValue) parameters.Add($"courses[{i}][showgrades]", course.ShowGrades.Value);
            if (course.NewsItems.HasValue) parameters.Add($"courses[{i}][newsitems]", course.NewsItems.Value);
            if (course.StartDate.HasValue) parameters.Add($"courses[{i}][startdate]", course.StartDate.Value);
            if (course.EndDate.HasValue) parameters.Add($"courses[{i}][enddate]", course.EndDate.Value);
            if (course.NumSections.HasValue) parameters.Add($"courses[{i}][numsections]", course.NumSections.Value);
            if (course.MaxBytes.HasValue) parameters.Add($"courses[{i}][maxbytes]", course.MaxBytes.Value);
            if (course.ShowReports.HasValue) parameters.Add($"courses[{i}][showreports]", course.ShowReports.Value);
            if (course.Visible.HasValue) parameters.Add($"courses[{i}][visible]", course.Visible.Value);
            if (course.GroupMode.HasValue) parameters.Add($"courses[{i}][groupmode]", course.GroupMode.Value);
            if (course.GroupModeForce.HasValue) parameters.Add($"courses[{i}][groupmodeforce]", course.GroupModeForce.Value);
            if (course.DefaultGroupId.HasValue) parameters.Add($"courses[{i}][defaultgroupid]", course.DefaultGroupId.Value);
        }

        return _client.ExecuteAsync<NewCourse[]>(new MoodleRequest
        {
            Function = MoodleFunctions.Course.CreateCourses,
            Method = HttpMethod.Post,
            Parameters = parameters
        });
    }

    /// <summary>
    /// Update an existing course.
    /// </summary>
    /// <param name="id">The ID of the course to update.</param>
    /// <param name="updates">A dictionary of fields to update.</param>
    /// <returns>The update result.</returns>
    public Task<MoodleResult<UpdateCourseRoot>> UpdateCoursesAsync(int id, IDictionary<string, object> updates)
    {
        var parameters = new Dictionary<string, object>
        {
            { "courses[0][id]", id }
        };

        foreach (var update in updates)
        {
            parameters.Add($"courses[0][{update.Key}]", update.Value);
        }

        return _client.ExecuteAsync<UpdateCourseRoot>(new MoodleRequest
        {
            Function = MoodleFunctions.Course.UpdateCourses,
            Method = HttpMethod.Post,
            Parameters = parameters
        });
    }
}
