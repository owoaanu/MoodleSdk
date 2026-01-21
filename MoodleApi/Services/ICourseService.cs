using MoodleApi.Models;
using MoodleSdk.Core;

namespace MoodleSdk.Services;

/// <summary>
/// Provides access to course and category management functions in the Moodle API.
/// </summary>
public interface ICourseService
{
    Task<MoodleResult<Category[]>> GetCategoriesAsync(string criteriaKey, string criteriaValue, int addSubCategories = 1);
    Task<MoodleResult<Course[]>> GetCoursesAsync(int[]? options);
    Task<MoodleResult<MoodleCourseListResponse>> GetCoursesByFieldAsync(string field, string value);
    Task<MoodleResult<Content[]>> GetContentsAsync(int courseId);
    Task<MoodleResult<EnrolledUser[]>> GetEnrolledUsersByCourseAsync(int courseId);
    Task<MoodleResult<NewCourse[]>> CreateCoursesAsync(params MoodleSdk.Models.CreateCourseRequest[] courses);
    Task<MoodleResult<UpdateCourseRoot>> UpdateCoursesAsync(int id, IDictionary<string, object> updates);
}
