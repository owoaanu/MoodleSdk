namespace MoodleSdk.Core;

/// <summary>
/// Contains all Moodle Web Service function names used by the SDK.
/// Centralizing these strings ensures consistency and easy maintenance.
/// </summary>
public static class MoodleFunctions
{
    /// <summary>
    /// System and site information functions.
    /// </summary>
    public static class System
    {
        public const string GetSiteInfo = "core_webservice_get_site_info";
    }

    /// <summary>
    /// User management functions.
    /// </summary>
    public static class User
    {
        public const string GetUsers = "core_user_get_users";
        public const string GetUsersByField = "core_user_get_users_by_field";
        public const string CreateUsers = "core_user_create_users";
        public const string UpdateUsers = "core_user_update_users";
        public const string DeleteUsers = "core_user_delete_users";
    }

    /// <summary>
    /// Course and category functions.
    /// </summary>
    public static class Course
    {
        public const string GetCategories = "core_course_get_categories";
        public const string GetCourses = "core_course_get_courses";
        public const string GetCoursesByField = "core_course_get_courses_by_field";
        public const string GetContents = "core_course_get_contents";
        public const string UpdateCourses = "core_course_update_courses";
        public const string CreateCourses = "core_course_create_courses";
    }

    /// <summary>
    /// Enrolment functions.
    /// </summary>
    public static class Enrolment
    {
        public const string GetUsersCourses = "core_enrol_get_users_courses";
        public const string GetEnrolledUsers = "core_enrol_get_enrolled_users";
        public const string ManualEnrolUsers = "enrol_manual_enrol_users";
    }

    /// <summary>
    /// Role management functions.
    /// </summary>
    public static class Role
    {
        public const string AssignRoles = "core_role_assign_roles";
        public const string UnassignRoles = "core_role_unassign_roles";
    }

    /// <summary>
    /// Group management functions.
    /// </summary>
    public static class Group
    {
        public const string CreateGroups = "core_group_create_groups";
        public const string GetGroups = "core_group_get_groups";
        public const string GetCourseGroups = "core_group_get_course_groups";
        public const string AddGroupMembers = "core_group_add_group_members";
        public const string DeleteGroupMembers = "core_group_delete_group_members";
    }

    /// <summary>
    /// Grade management functions.
    /// </summary>
    public static class Grade
    {
        public const string GetGrades = "core_grades_get_grades";
    }

    /// <summary>
    /// Calendar and event functions.
    /// </summary>
    public static class Calendar
    {
        public const string GetCalendarEvents = "core_calendar_get_calendar_events";
        public const string CreateCalendarEvents = "core_calendar_create_calendar_events";
        public const string DeleteCalendarEvents = "core_calendar_delete_calendar_events";
    }
}
