using MoodleSdk.Extensions;
using MoodleApi.Models;
using MoodleApi.Models.Responses;
using System.Net;
using System.Text;
using System.Linq;
using MoodleSdk;

namespace MoodleApi;

[Obsolete("Use IMoodleClient and the service-based architecture instead. This class will be removed in a future major version.")]
public class Moodle
{
    private readonly HttpClient _httpClient;
    private IMoodleClient? _sdkClient;
    private MoodleSdk.Core.MoodleOptions? _sdkOptions;
    #region Properties

    /// <summary>
    /// This property sets you Api token.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Repressents if the token is set.
    /// </summary>
    private bool TokenIsSet => string.IsNullOrEmpty(Token) is false;

    public Uri? Host { get; set; }

    /// <summary>
    /// Represents if the host address is set.
    /// </summary>
    private bool HostIsSet => Host is not null;

    #endregion

    #region SDK Delegation

    private IMoodleClient GetSdkClient()
    {
        if (_sdkClient == null)
        {
            if (Host == null) throw new InvalidOperationException("Host is not set. Call SetHost first.");
            _sdkOptions = new MoodleSdk.Core.MoodleOptions
            {
                BaseUrl = Host,
                DefaultToken = Token ?? ""
            };
            // Use a local HttpClient or the injected one
            _sdkClient = new MoodleSdk.MoodleClient(_httpClient, _sdkOptions, Enumerable.Empty<MoodleSdk.Hooks.IMoodleClientHook>());
        }
        return _sdkClient;
    }

    #endregion

    #region Constructors

    public Moodle(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    #endregion

    #region Methods

    #region Helper

    private int DateTimeToUnixTimestamp(DateTime dateTime)
    {
        return Convert.ToInt32((TimeZoneInfo.ConvertTimeToUtc(dateTime) - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
    }

    private string ParseFormat(MoodleFormat format)
    {
        switch (format)
        {
            case MoodleFormat.JSON:
                return "json";
            case MoodleFormat.XML:
                return "xml";
        }
        throw new ArgumentOutOfRangeException("format");
    }

    private string ParseMethod(MoodleMethod method)
    {
        switch (method)
        {
            case MoodleMethod.CoreWebserviceGetSiteInfo:
                return "core_webservice_get_site_info";
            case MoodleMethod.CoreUserGetUsers:
                return "core_user_get_users";
            case MoodleMethod.CoreUserGetUsersByField:
                return "core_user_get_users_by_field";
            case MoodleMethod.CoreEnrolGetUsersCourses:
                return "core_enrol_get_users_courses";
            case MoodleMethod.CoreUserCreateUsers:
                return "core_user_create_users";
            case MoodleMethod.CoreUserUpdateUsers:
                return "core_user_update_users";
            case MoodleMethod.CoreUserDeleteUsers:
                return "core_user_delete_users";
            case MoodleMethod.CoreRoleAssignRoles:
                return "core_role_assign_roles";
            case MoodleMethod.CoreRoleUnassignRoles:
                return "core_role_unassign_roles";
            case MoodleMethod.EnrolManualEnrolUsers:
                return "enrol_manual_enrol_users";
            case MoodleMethod.CoreGroupAddGroupMembers:
                return "core_group_add_group_members";
            case MoodleMethod.CoreGroupDeleteGroupMembers:
                return "core_group_delete_group_members";
            case MoodleMethod.CoreCourseGetCategories:
                return "core_course_get_categories";
            case MoodleMethod.CoreCourseGetCourses:
                return "core_course_get_courses";
            case MoodleMethod.CoreCourseGetContents:
                return "core_course_get_contents";
            case MoodleMethod.CoreGroupGetGroups:
                return "core_group_get_groups";
            case MoodleMethod.CoreGroupGetCourseGroups:
                return "core_group_get_course_groups";
            case MoodleMethod.CoreEnrolGetEnrolledUsers:
                return "core_enrol_get_enrolled_users";
            case MoodleMethod.CoreCourseCreateCourses:
                return "core_course_create_courses";
            case MoodleMethod.CoreCourseUpdateCourses:
                return "core_course_update_courses";
            case MoodleMethod.CoreGradesGetGrades:
                return "core_grades_get_grades";
            case MoodleMethod.CoreGradesUpdateGrades:
                return "core_grades_update_grades";
            case MoodleMethod.CoreGradingGetDefinitions:
                return "core_grading_get_definitions";
            case MoodleMethod.CoreCalendarGetCalendarEvents:
                return "core_calendar_get_calendar_events";
            case MoodleMethod.CoreCalendarCreateCalendarEvents:
                return "core_calendar_create_calendar_events";
            case MoodleMethod.CoreCalendarDeleteCalendarEvents:
                return "core_calendar_delete_calendar_events";
            case MoodleMethod.Default:
                return "";
        }
        throw new ArgumentOutOfRangeException("method");
    }

    private StringBuilder GetBaseQuery(MoodleMethod moodleMethod, MoodleFormat moodleFormat = MoodleFormat.JSON)
    {
        if (TokenIsSet is false)
            throw new ArgumentNullException("Token is not set");

        StringBuilder query = new StringBuilder("webservice/rest/server.php?wstoken=");
        query.Append(Token)
            .AppendFilterQuery("&wsfunction=", ParseMethod(moodleMethod))
            .AppendFilterQuery("&moodlewsrestformat=", ParseFormat(moodleFormat));

        return query;
    }

    #endregion

    #region Authentications

    /// <summary>
    /// Returns your Api Token needed to make any calls
    /// <para />
    /// service shortname - The service shortname is usually hardcoded in the pre-build service (db/service.php files).
    /// Moodle administrator will be able to edit shortnames for service created on the fly: MDL-29807.
    /// If you want to use the Mobile service, its shortname is moodle_mobile_app. Also useful to know,
    /// the database shortname field can be found in the table named external_services.
    /// </summary>
    /// <param names="userName"></param>
    /// <param names="password"></param>
    /// <param names="serviceHostName"></param>
    /// <returns></returns>
    public async Task<AuthentiactionResult> Login(string userName, string password, string serviceHostName = "moodle_mobile_app")
    {
        string query = $"login/token.php?username={userName}&password={password}&service={serviceHostName}";

        var result = await GetAuth<AuthToken>(query);

        if (result.Data?.Token.HasNoValue() ?? true)
        {
            return new AuthentiactionResult(result.Error);
        }
        else
        {
            Token = result.Data?.Token;
            return new AuthentiactionResult();
        }
    }

    #endregion

    #region System actions
    /// <summary>
    /// This API will return information about the site, web services users, and authorized API actions. This call is useful for getting site information and the capabilities of the web service user. 
    /// </summary>
    /// <param names="serviceHostNames">Returns information about a particular service.</param>
    /// <returns></returns>
    public async Task<MoodleResponse<SiteInfo>> GetSiteInfo(string serviceShortName = "")
    {
        var sdk = GetSdkClient();
        var result = await sdk.System.GetSiteInfoAsync(serviceShortName);
        return new MoodleResponse<SiteInfo>(result.IsSuccess, result.Data, null, ToLegacyError(result.Error));
    }

    #endregion

    #region User Actions

    /// <summary>
    /// Search for users matching the parameters of the call. This call will return matching user accounts with profile fields.
    ///  The key/value pairs to be considered in user search. Values can not be empty. Specify different keys only once
    ///  (fullname =&gt; 'user1', auth =&gt; 'manual', ...) - key occurences are forbidden. The search is executed with AND operator on the criterias.
    ///  Invalid criterias (keys) are ignored, the search is still executed on the valid criterias. You can search without criteria,
    ///  but the function is not designed for it. It could very slow or timeout. The function is designed to search some specific users.
    /// <para />
    /// "id" (int) matching user id<para />
    ///"lastname" (string) user last names (Note: you can use % for searching but it may be considerably slower!)<para />
    ///"firstname" (string) user first names (Note: you can use % for searching but it may be considerably slower!)<para />
    ///"idnumber" (string) matching user idnumber<para />
    ///"username" (string) matching user username<para />
    ///"email" (string) user email (Note: you can use % for searching but it may be considerably slower!)<para />
    ///"auth" (string) matching user auth plugin<para />
    /// </summary>
    /// <param names="criteriaKey">Key of the first search parameter.</param>
    /// <param names="criteriaValue">Value of the first search term.</param>
    /// <param names="criterias">criteria key and value of the second and other search parameter.</param>
    /// <returns></returns>
    public async Task<MoodleResponse<UsersData>> GetUsers(string criteriaKey, string criteriaValue, params (string? Key, string? Value)[] criterias)
    {
        var sdk = GetSdkClient();
        var result = await sdk.Users.GetUsersAsync(criteriaKey, criteriaValue, criterias.Select(c => (c.Key ?? "", c.Value ?? "")).ToArray());
        return new MoodleResponse<UsersData>(result.IsSuccess, result.Data, null, ToLegacyError(result.Error));
    }

    /// <summary>
    /// Retrieve users information for a specified unique field - If you want to do a user search, use GetUsers()
    /// 
    /// Avaiable Criteria:
    ///"id" (int) matching user id
    ///"lastname" (string) user last names (Note: you can use % for searching but it may be considerably slower!)
    ///"firstname" (string) user first names (Note: you can use % for searching but it may be considerably slower!)
    ///"idnumber" (string) matching user idnumber
    ///"username" (string) matching user username
    ///"email" (string) user email (Note: you can use % for searching but it may be considerably slower!)
    ///"auth" (string) matching user auth plugin
    /// </summary>
    /// <param names="field">Field of the search parameter.</param>
    /// <param names="criteriaValue">Values of the search term.</param>
    /// <returns></returns>
    public async Task<MoodleResponse<User>> GetUsersByField(string field, params string[] values)
    {
        var sdk = GetSdkClient();
        var result = await sdk.Users.GetUsersByFieldAsync(field, values);
        return new MoodleResponse<User>(result.IsSuccess, null, result.Data, ToLegacyError(result.Error));
    }

    /// <summary>
    /// Get the list of courses where a user is enrolled in 
    /// </summary>
    /// <param names="userId"></param>
    /// <returns></returns>
    public async Task<MoodleResponse<Cources>> GetUserCourses(int userId)
    {
        var sdk = GetSdkClient();
        var result = await sdk.Enrolments.GetUserCoursesAsync(userId);
        return new MoodleResponse<Cources>(result.IsSuccess, null, result.Data, ToLegacyError(result.Error));
    }

    /// <summary>
    /// Create a User.
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <param name="auth"></param>
    /// <param name="idNumber"></param>
    /// <param name="language"></param>
    /// <param name="calendartype"></param>
    /// <param name="theme"></param>
    /// <param name="timezone"></param>
    /// <param name="mailFormat"></param>
    /// <param name="description"></param>
    /// <param name="city"></param>
    /// <param name="country"></param>
    /// <param name="firstNamePhonetic"></param>
    /// <param name="lastNamePhonetic"></param>
    /// <param name="middleName"></param>
    /// <param name="alternateName"></param>
    /// <param name="preferencesType"></param>
    /// <param name="preferencesValue"></param>
    /// <param name="customFieldsType"></param>
    /// <param name="customFieldsValue"></param>
    /// <returns></returns>
    public async Task<MoodleResponse<NewUser>> CreateUser(string userName, string firstName, string lastName, string email, string password,
        string auth = "", string idNumber = "", string language = "", string calendartype = "", string theme = "",
        string timezone = "", string mailFormat = "", string description = "", string city = "", string country = "",
        string firstNamePhonetic = "", string lastNamePhonetic = "", string middleName = "", string alternateName = "",
        string preferencesType = "", string preferencesValue = "",
        string customFieldsType = "", string customFieldsValue = "")
    {
        var sdk = GetSdkClient();
        var request = new MoodleSdk.Models.CreateUserRequest
        {
            UserName = userName,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Password = password,
            Auth = auth,
            IdNumber = idNumber,
            Language = language,
            Theme = theme,
            Timezone = timezone,
            Description = description,
            City = city,
            Country = country,
            CalendarType = calendartype,
            MailFormat = mailFormat,
            FirstNamePhonetic = firstNamePhonetic,
            LastNamePhonetic = lastNamePhonetic,
            MiddleName = middleName,
            AlternateName = alternateName
        };

        if (!string.IsNullOrEmpty(preferencesType))
        {
            request.Preferences = new List<(string Name, string Value)> { (preferencesType, preferencesValue) };
        }

        if (!string.IsNullOrEmpty(customFieldsType))
        {
            request.CustomFields = new List<(string Type, string Value)> { (customFieldsType, customFieldsValue) };
        }

        var result = await sdk.Users.CreateUserAsync(request);
        return new MoodleResponse<NewUser>(result.IsSuccess, result.Data?[0], null, ToLegacyError(result.Error));
    }


    /// <summary>
    /// Updates a user
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userName"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <param name="auth"></param>
    /// <param name="idNumber"></param>
    /// <param name="language"></param>
    /// <param name="calendartye"></param>
    /// <param name="theme"></param>
    /// <param name="timezone"></param>
    /// <param name="mailFormat"></param>
    /// <param name="description"></param>
    /// <param name="city"></param>
    /// <param name="country"></param>
    /// <param name="firstNamePhonetic"></param>
    /// <param name="lastNamePhonetic"></param>
    /// <param name="middleName"></param>
    /// <param name="alternateName"></param>
    /// <param name="preferencesType"></param>
    /// <param name="preferencesValue"></param>
    /// <param name="customfieldsType"></param>
    /// <param name="customfieldsValue"></param>
    /// <returns></returns>
    public async Task<MoodleResponse<Success>> UpdateUser(int id, string userName = "", string firstName = "", string lastName = "",
        string email = "", string password = "", string auth = "", string idNumber = "", string language = "",
        string calendartype = "", string theme = "", string timezone = "", string mailFormat = "", string description = "", string city = "", string country = "",
        string firstNamePhonetic = "", string lastNamePhonetic = "", string middleName = "", string alternateName = "",
        string preferencesType = "", string preferencesValue = "",
        string customfieldsType = "", string customfieldsValue = "")
    {
        var sdk = GetSdkClient();
        var request = new MoodleSdk.Models.CreateUserRequest
        {
            UserName = userName,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Password = password,
            Auth = auth,
            IdNumber = idNumber,
            Language = language,
            Theme = theme,
            Timezone = timezone,
            Description = description,
            City = city,
            Country = country,
            CalendarType = calendartype,
            MailFormat = mailFormat,
            FirstNamePhonetic = firstNamePhonetic,
            LastNamePhonetic = lastNamePhonetic,
            MiddleName = middleName,
            AlternateName = alternateName
        };

        if (!string.IsNullOrEmpty(preferencesType))
        {
            request.Preferences = new List<(string Name, string Value)> { (preferencesType, preferencesValue) };
        }

        if (!string.IsNullOrEmpty(customfieldsType))
        {
            request.CustomFields = new List<(string Type, string Value)> { (customfieldsType, customfieldsValue) };
        }

        var result = await sdk.Users.UpdateUserAsync(id, request);
        return new MoodleResponse<Success>(result.IsSuccess, result.Data, null, ToLegacyError(result.Error));
    }

    /// <summary>
    /// elete a user
    /// </summary>
    /// <param names="id"></param>
    /// <returns></returns>
    public async Task<MoodleResponse<Success>> DeleteUser(int id)
    {
        var sdk = GetSdkClient();
        var result = await sdk.Users.DeleteUsersAsync(id);
        return new MoodleResponse<Success>(result.IsSuccess, result.Data, null, ToLegacyError(result.Error));
    }

    /// <summary>
    /// Manual role assignments. This call should be made in an array.
    /// </summary>
    /// <param name="roleId">
    /// <summary>Role to assign to the user</summary>
    /// </param>
    /// <param name="userId"></param>
    /// <param name="contextId"></param>
    /// <param name="contextLevel"></param>
    /// <param name="instanceId"></param>
    /// <returns></returns>
    public async Task<MoodleResponse<Success>> AssignRoles(int roleId, int userId, string contextId = "", string contextLevel = "", int? instanceId = null)
    {
        var sdk = GetSdkClient();
        var result = await sdk.Enrolments.AssignRoleAsync(roleId, userId, contextId, contextLevel, instanceId);
        return new MoodleResponse<Success>(result.IsSuccess, result.Data, null, ToLegacyError(result.Error));
    }

    /// <summary>
    /// Manual role unassignments.
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="userId"></param>
    /// <param name="contextId"></param>
    /// <param name="contextLevel"></param>
    /// <param name="instanceId"></param>
    /// <returns></returns>
    public async Task<MoodleResponse<Success>> UnassignRoles(int roleId, int userId, string contextId = "", string contextLevel = "", int? instanceId = null)
    {
        var sdk = GetSdkClient();
        var result = await sdk.Enrolments.UnassignRoleAsync(roleId, userId, contextId, contextLevel, instanceId);
        return new MoodleResponse<Success>(result.IsSuccess, result.Data, null, ToLegacyError(result.Error));
    }

    #endregion

    #region Course Enrollment Actions

    public async Task<MoodleResponse<Success>> EnrolUser(int roleId, int userId, int courceId, int? timeStart = null, int? timeEnd = null, int? suspend = null)
    {
        var sdk = GetSdkClient();
        // Manual mapping for now since IEnrolmentService.EnrolUserAsync is not implemented yet.
        // I'll use Custom for now to keep Moodle.cs delegation moving.
        var parameters = new Dictionary<string, object>
        {
            { "enrolments[0][roleid]", roleId },
            { "enrolments[0][userid]", userId },
            { "enrolments[0][courseid]", courceId } // Note: original code had 'courceid' typo which matches Moodle API
        };
        if (timeStart.HasValue) parameters.Add("enrolments[0][timestart]", timeStart.Value);
        if (timeEnd.HasValue) parameters.Add("enrolments[0][timeend]", timeEnd.Value);
        if (suspend.HasValue) parameters.Add("enrolments[0][suspend]", suspend.Value);

        var result = await sdk.Custom.CallAsync<Success>("enrol_manual_enrol_users", HttpMethod.Post, parameters);
        return new MoodleResponse<Success>(result.IsSuccess, result.Data, null, ToLegacyError(result.Error));
    }

    public async Task<MoodleResponse<Success>> AddGroupMember(int groupId, int userId)
    {
        var sdk = GetSdkClient();
        var result = await sdk.Groups.AddGroupMembersAsync((groupId, userId));
        return new MoodleResponse<Success>(result.IsSuccess, result.Data, null, ToLegacyError(result.Error));
    }

    public async Task<MoodleResponse<Success>> DeleteGroupMember(int groupId, int userId)
    {
        var sdk = GetSdkClient();
        var result = await sdk.Groups.DeleteGroupMembersAsync((groupId, userId));
        return new MoodleResponse<Success>(result.IsSuccess, result.Data, null, ToLegacyError(result.Error));
    }


    #endregion

    #region Course Actions

    /// <summary>
    /// Get a listing of categories in the system.
    /// </summary>
    /// <param names="criteriaKey">
    /// <summary>
    /// criteria[0][key] - The category column to search, expected keys (value format) are:"id" (int) the category id,"names" (string)
    ///  the category names,"parent" (int) the parent category id,"idnumber" (string) category idnumber - user must have 'moodle/category:manage'
    ///  to search on idnumber,"visible" (int) whether the returned categories must be visible or hidden.
    ///  If the key is not passed, then the function return all categories that the user can see. - user must have 'moodle/category:manage'
    ///  or 'moodle/category:viewhiddencategories' to search on visible,"theme" (string) only return the categories having this theme
    ///  - user must have 'moodle/category:manage' to search on theme
    /// </summary>
    /// </param>
    /// <param names="criteriaValue"><summary>Criteria[0][value] - The value to match</summary></param>
    /// <param names="addSubCategories"><summary>Return the sub categories infos (1 - default) otherwise only the category info (0)</summary></param>
    /// <returns></returns>
    public async Task<MoodleResponse<Category>> GetCategories(string criteriaKey, string criteriaValue, int addSubCategories = 1)
    {
        var sdk = GetSdkClient();
        var result = await sdk.Courses.GetCategoriesAsync(criteriaKey, criteriaValue, addSubCategories);
        // Note: SDK returns Category[], legacy wrapper expects single Category (which usually contains the array via deserialization of the root).
        // Actually, Categories usually return an array. Let's check Category.cs structure.
        return new MoodleResponse<Category>(result.IsSuccess, result.Data?.FirstOrDefault(), result.Data, ToLegacyError(result.Error));
    }

    /// <summary>
    /// Get a listing of courses in the system.
    /// </summary>
    /// <param names="options"><summary>List of course id.If empty return all courses except front page course.</summary></param>
    /// <returns></returns>
    public async Task<MoodleResponse<Course>> GetCourses(int? options = null)
    {
        var sdk = GetSdkClient();
        var result = await sdk.Courses.GetCoursesAsync(options.HasValue ? [options.Value] : null);
        return new MoodleResponse<Course>(result.IsSuccess, result.Data?.FirstOrDefault(), result.Data, ToLegacyError(result.Error));
    }

    /// <summary>
    /// Get course contents
    /// </summary>
    /// <param names="course_id"><summary>Course Id</summary></param>
    /// <returns></returns>
    public async Task<MoodleResponse<Content>> GetContents(int courseId)
    {
        var sdk = GetSdkClient();
        var result = await sdk.Courses.GetContentsAsync(courseId);
        return new MoodleResponse<Content>(result.IsSuccess, result.Data?.FirstOrDefault(), result.Data, ToLegacyError(result.Error));
    }

    /// <summary>
    /// Returns group details. 
    /// </summary>
    /// <param names="groupId">Group Id</param>
    /// <returns></returns>
    public async Task<MoodleResponse<Group>> GetGroup(int groupId)
    {
        var sdk = GetSdkClient();
        var result = await sdk.Groups.GetGroupsAsync(groupId);
        return new MoodleResponse<Group>(result.IsSuccess, result.Data?.FirstOrDefault(), result.Data, ToLegacyError(result.Error));
    }
    /// <summary>
    /// Returns group details. 
    /// </summary>
    /// <param names="groupIds"><summary>Group Ids</summary></param>
    /// <returns></returns>
    public async Task<MoodleResponse<Group>> GetGroups(int[] groupIds)
    {
        var sdk = GetSdkClient();
        var result = await sdk.Groups.GetGroupsAsync(groupIds);
        return new MoodleResponse<Group>(result.IsSuccess, result.Data?.FirstOrDefault(), result.Data, ToLegacyError(result.Error));
    }

    /// <summary>
    /// Returns all groups in specified course.
    /// </summary>
    /// <param names="courseId"><summary>Course Id</summary></param>
    /// <returns></returns>
    public async Task<MoodleResponse<Group>> GetCourseGroups(int courseId)
    {
        var sdk = GetSdkClient();
        var result = await sdk.Groups.GetCourseGroupsAsync(courseId);
        return new MoodleResponse<Group>(result.IsSuccess, result.Data?.FirstOrDefault(), result.Data, ToLegacyError(result.Error));
    }

    /// <summary>
    /// Get enrolled users by course id. 
    /// </summary>
    /// <param names="courseId"></param>
    /// <returns></returns>
    public async Task<MoodleResponse<EnrolledUser>> GetEnrolledUsersByCourse(int courseId)
    {
        var sdk = GetSdkClient();
        var result = await sdk.Courses.GetEnrolledUsersByCourseAsync(courseId);
        return new MoodleResponse<EnrolledUser>(result.IsSuccess, result.Data?.FirstOrDefault(), result.Data, ToLegacyError(result.Error));
    }

    /// <summary>
    /// Create new course
    /// </summary>
    /// <param names="fullName"><summary>Full names of the course</summary></param>
    /// <param names="shortName"><summary>Shortname of the course</summary></param>
    /// <param names="categoryId"><summary>Category ID of the course</summary></param>
    /// <param names="idNumber"><summary>Optional //id number</summary></param>
    /// <param names="summary"><summary>Optional //summary</summary></param>
    /// <param names="summaryFormat"><summary>Default to "1" //summary format (1 = HTML, 0 = MOODLE, 2 = PLAIN or 4 = MARKDOWN)</summary></param>
    /// <param names="format"><summary>Default to "topics" //course format: weeks, topics, social, site,..</summary></param>
    /// <param names="showGrades"><summary>Default to "0" //1 if grades are shown, otherwise 0</summary></param>
    /// <param names="newsItems"><summary>Default to "0" //number of recent items appearing on the course page</summary></param>
    /// <param names="startDate"><summary>Optional //timestamp when the course start</summary></param>
    /// <param names="numSections"><summary>Optional //(deprecated, use courseformatoptions) number of weeks/topics</summary></param>
    /// <param names="maxBytes"><summary>Default to "104857600" //largest size of file that can be uploaded into the course</summary></param>
    /// <param names="showReports"><summary>Default to "1" //are activity report shown (yes = 1, no =0)</summary></param>
    /// <param names="visible"><summary>Optional //1: available to student, 0:not available</summary></param>
    /// <param names="hiddenSections"><summary>Optional //(deprecated, use courseformatoptions) How the hidden sections in the course are displayed to students</summary></param>
    /// <param names="groupMode"><summary>Default to "0" //no group, separate, visible</summary></param>
    /// <param names="groupModeForce"><summary>Default to "0" //1: yes, 0: no</summary></param>
    /// <param names="defaultGroupingId"><summary>Default to "0" //default grouping id</summary></param>
    /// <param names="enableCompletion"><summary>Optional //Enabled, control via completion and activity settings. Disabled, not shown in activity settings.</summary></param>
    /// <param names="completeNotify"><summary>Optional //1: yes 0: no</summary></param>
    /// <param names="language"><summary>//forced course language</summary></param>
    /// <param names="forceTheme"><summary>Optional //names of the force theme</summary></param>
    /// <param names="courcCourseformatoption"><summary>Optional //additional options for particular course format list of ( object { names string //course format option names
    ///value string //course format option value } )} )</summary></param>
    /// <returns></returns>
    public async Task<MoodleResponse<NewCourse>> CreateCourse(string fullName, string shortName, int categoryId,
        string idNumber = "", string summary = "", int summaryFormat = 1, string format = "", int showGrades = 0, int newsItems = 0,
        DateTime startdate = default, int numSections = int.MaxValue, int maxBytes = 104857600, int showReports = 1,
        int visible = 0, int hiddenSections = int.MaxValue, int groupMode = 0,
        int groupModeForce = 0, int defaultGroupingId = 0, int enableCompletion = int.MaxValue,
        int completeNotify = 0, string language = "", string forceTheme = "",
        string courcCourseformatoption = ""/*not implemented*/)
    {
        var sdk = GetSdkClient();
        var request = new MoodleSdk.Models.CreateCourseRequest
        {
            FullName = fullName,
            ShortName = shortName,
            CategoryId = categoryId,
            IdNumber = idNumber,
            Summary = summary,
            SummaryFormat = summaryFormat,
            Format = format,
            ShowGrades = showGrades,
            NewsItems = newsItems,
            StartDate = startdate == default ? null : (int)DateTimeToUnixTimestamp(startdate),
            NumSections = numSections == int.MaxValue ? null : numSections,
            MaxBytes = maxBytes,
            ShowReports = showReports,
            Visible = visible,
            GroupMode = groupMode,
            GroupModeForce = groupModeForce,
            DefaultGroupId = defaultGroupingId
        };

        var result = await sdk.Courses.CreateCoursesAsync(request);
        return new MoodleResponse<NewCourse>(result.IsSuccess, result.Data?.FirstOrDefault(), result.Data, ToLegacyError(result.Error));
    }

    /// <summary>
    /// Create new courses
    /// </summary>
    /// <param name="courses"></param>
    /// <returns></returns>
    public async Task<MoodleResponse<NewCourse>> CreateCourses((string FullName, string ShortName, int CategoryId)[] courses)
    {
        var sdk = GetSdkClient();
        var requests = courses.Select(c => new MoodleSdk.Models.CreateCourseRequest
        {
            FullName = c.FullName,
            ShortName = c.ShortName,
            CategoryId = c.CategoryId
        }).ToArray();

        var result = await sdk.Courses.CreateCoursesAsync(requests);
        return new MoodleResponse<NewCourse>(result.IsSuccess, result.Data?.FirstOrDefault(), result.Data, ToLegacyError(result.Error));
    }

    public async Task<MoodleResponse<UpdateCourseRoot>> UpdateCourse(int id, string fullName = "", string shortName = "", int categoryId = int.MaxValue,
        string idNumber = "", string summary = "", int summaryFormat = 1, string format = "", int showGrades = 0, int newsItems = 0,
        DateTime startdate = default, int numsections = int.MaxValue, int maxbytes = 104857600, int showreports = 1,
        int visible = 0, int hiddenSections = int.MaxValue, int groupMode = 0,
        int groupModeForce = 0, int defaultGroupingId = 0, int enableCompletion = int.MaxValue,
        int completenotify = 0, string language = "", string forceTheme = "",
        string courcCourseformatoption = ""/*not implemented*/)
    {
        var sdk = GetSdkClient();
        var updates = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(fullName)) updates.Add("fullname", fullName);
        if (!string.IsNullOrEmpty(shortName)) updates.Add("shortname", shortName);
        if (categoryId != int.MaxValue) updates.Add("categoryid", categoryId);
        if (!string.IsNullOrEmpty(idNumber)) updates.Add("idnumber", idNumber);
        if (!string.IsNullOrEmpty(summary)) updates.Add("summary", summary);
        if (summaryFormat != 1) updates.Add("summaryformat", summaryFormat);
        if (!string.IsNullOrEmpty(format)) updates.Add("format", format);
        if (showGrades != 0) updates.Add("showgrades", showGrades);
        if (startdate != default) updates.Add("startdate", DateTimeToUnixTimestamp(startdate));
        if (newsItems != 0) updates.Add("newsitems", newsItems);
        if (numsections != int.MaxValue) updates.Add("numsections", numsections);
        if (maxbytes != 104857600) updates.Add("maxbytes", maxbytes);
        if (showreports != 1) updates.Add("showreports", showreports);
        if (visible != 0) updates.Add("visible", visible);
        if (hiddenSections != int.MaxValue) updates.Add("hiddensections", hiddenSections);
        if (groupMode != 0) updates.Add("groupmode", groupMode);
        if (groupModeForce != 0) updates.Add("groupmodeforce", groupModeForce);
        if (defaultGroupingId != 0) updates.Add("defaultgroupingid", defaultGroupingId);
        if (enableCompletion != int.MaxValue) updates.Add("enablecompletion", enableCompletion);
        if (completenotify != 0) updates.Add("completenotify", completenotify);
        if (!string.IsNullOrEmpty(language)) updates.Add("lang", language);
        if (!string.IsNullOrEmpty(forceTheme)) updates.Add("forcetheme", forceTheme);

        var result = await sdk.Courses.UpdateCoursesAsync(id, updates);
        return new MoodleResponse<UpdateCourseRoot>(result.IsSuccess, result.Data, null, ToLegacyError(result.Error));
    }

    #endregion

    #region Grade Actions

    /// <summary>
    /// Returns grade item details and optionally student grades. 
    /// </summary>
    /// <param names="criteria_key"></param>
    /// <param names="criteria_value"></param>
    /// <param names="addSubCategories"></param>
    /// <returns></returns>
    public async Task<MoodleResponse<Category>> GetGrades(int courseId, string component = "", int activityid = int.MaxValue, string[]? userIds = null)
    {
        var sdk = GetSdkClient();
        var result = await sdk.Grades.GetGradesAsync(courseId, component, activityid, userIds);
        return new MoodleResponse<Category>(result.IsSuccess, result.Data, null, ToLegacyError(result.Error));
    }



    #endregion

    #region Calander Actions


    public async Task<MoodleResponse<ListOfEvents>> GetCalanderEvents(int[]? groupids = null, int[]? courseIds = null, int[]? eventIds = null)
    {
        var sdk = GetSdkClient();
        var result = await sdk.Calendar.GetCalendarEventsAsync(groupids, courseIds, eventIds);
        return new MoodleResponse<ListOfEvents>(result.IsSuccess, result.Data, null, ToLegacyError(result.Error));
    }


    public async Task<MoodleResponse<ListOfEvents>> CreateCalanderEvents(string[] names, string[]? descriptions = null,
         int[]? formats = null, int[]? groupIds = null, int[]? courseIds = null, int[]? repeats = null,
         string[]? eventTypes = null, DateTime[]? timeStarts = null, TimeSpan[]? timeDurations = null,
         int[]? visible = null, int[]? sequences = null)
    {
        var sdk = GetSdkClient();
        var events = new List<Dictionary<string, object>>();
        for (int i = 0; i < names.Length; i++)
        {
            var e = new Dictionary<string, object> { { "name", names[i] } };
            if (groupIds != null && i < groupIds.Length) e.Add("groupid", groupIds[i]);
            if (courseIds != null && i < courseIds.Length) e.Add("courseid", courseIds[i]);
            if (descriptions != null && i < descriptions.Length) e.Add("description", descriptions[i]);
            if (formats != null && i < formats.Length) e.Add("format", formats[i]);
            if (repeats != null && i < repeats.Length) e.Add("repeat", repeats[i]);
            if (eventTypes != null && i < eventTypes.Length) e.Add("eventtype", eventTypes[i]);
            if (timeStarts != null && i < timeStarts.Length) e.Add("timestart", DateTimeToUnixTimestamp(timeStarts[i]));
            if (timeDurations != null && i < timeDurations.Length) e.Add("timeduration", (int)timeDurations[i].TotalSeconds);
            if (visible != null && i < visible.Length) e.Add("visible", visible[i]);
            if (sequences != null && i < sequences.Length) e.Add("sequence", sequences[i]);
            events.Add(e);
        }

        var result = await sdk.Calendar.CreateCalendarEventsAsync(events);
        return new MoodleResponse<ListOfEvents>(result.IsSuccess, result.Data, null, ToLegacyError(result.Error));
    }


    public async Task<MoodleResponse<ListOfEvents>> DeleteCalanderEvents(int[]? eventIds, int[]? repeats, string[]? descriptions = null)
    {
        var sdk = GetSdkClient();
        var events = new List<Dictionary<string, object>>();
        int count = Math.Max(eventIds?.Length ?? 0, repeats?.Length ?? 0);
        for (int i = 0; i < count; i++)
        {
            var e = new Dictionary<string, object>();
            if (eventIds != null && i < eventIds.Length) e.Add("eventid", eventIds[i]);
            if (repeats != null && i < repeats.Length) e.Add("repeat", repeats[i]);
            if (descriptions != null && i < descriptions.Length) e.Add("description", descriptions[i]);
            events.Add(e);
        }

        var result = await sdk.Calendar.DeleteCalendarEventsAsync(events);
        return new MoodleResponse<ListOfEvents>(result.IsSuccess, result.Data, null, ToLegacyError(result.Error));
    }

    #endregion

    #region Group Actions

    public async Task<MoodleResponse<Group>> CreateGroups(string[]? names = null, int[]? courseids = null, string[]? descriptions = null,
        int[]? descriptionFormats = null, string[]? enrolmentKeys = null, string[]? idNumbers = null)
    {
        var sdk = GetSdkClient();
        var groups = new List<Dictionary<string, object>>();
        int count = names?.Length ?? 0;
        for (int i = 0; i < count; i++)
        {
            var g = new Dictionary<string, object> { { "name", names![i] } };
            if (courseids != null && i < courseids.Length) g.Add("courseid", courseids[i]);
            if (descriptions != null && i < descriptions.Length) g.Add("description", descriptions[i]);
            if (descriptionFormats != null && i < descriptionFormats.Length) g.Add("descriptionformat", descriptionFormats[i]);
            if (enrolmentKeys != null && i < enrolmentKeys.Length) g.Add("enrolmentkey", enrolmentKeys[i]);
            if (idNumbers != null && i < idNumbers.Length) g.Add("idnumber", idNumbers[i]);
            groups.Add(g);
        }

        var result = await sdk.Groups.CreateGroupsAsync(groups);
        return new MoodleResponse<Group>(result.IsSuccess, result.Data?.FirstOrDefault(), result.Data, ToLegacyError(result.Error));
    }

    #endregion

    #region Getters

    private async Task<AuthentiactionResponse<T>> GetAuth<T>(string query) where T : IDataModel
    {
        if (HostIsSet is false)
            throw new Exception("Host is not set");

        if (Host!.Scheme == "https")
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        var requestUri = new Uri(Host, query);
        var data = await _httpClient.GetStringAsync(requestUri);
        return new AuthentiactionResponse<T>(data);
    }


    private async Task<MoodleResponse<T>> Get<T>(string query) where T : IDataModel
    {
        if (HostIsSet is false)
            throw new Exception("Host is not set");

        if (Host!.Scheme == "https")
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        var requestUri = new Uri(Host, query);
        var data = await _httpClient.GetStringAsync(requestUri);
        return new MoodleResponse<T>(data);
    }

    private async Task<MoodleResponse<T>> Get<T>(StringBuilder query) where T : IDataModel
    {
        return await Get<T>(query.ToString());
    }
    #endregion

    #region Setters

    public void SetHost(string url)
    {
        Host = new Uri(url);
    }

    #endregion

    private MoodleApi.Models.Error? ToLegacyError(MoodleSdk.Core.MoodleError? error)
    {
        if (error == null) return null;
        return new MoodleApi.Models.Error
        {
            ErrorCode = error.ErrorCode,
            Exception = error.Exception,
            Message = error.Message,
            DebugInfo = error.DebugInfo
        };
    }

    #endregion
}