using System.Text.Json;
using MoodleSdk.Extensions;
using MoodleSdk.Core;
using MoodleSdk.Services;
using MoodleSdk.Exceptions;

namespace MoodleSdk;

/// <summary>
/// Default implementation of IMoodleClient.
/// Handles the plumbing of Web Service requests to Moodle.
/// </summary>
public sealed class MoodleClient : IMoodleClient
{
    private readonly HttpClient _httpClient;
    private readonly MoodleOptions _options;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IEnumerable<MoodleSdk.Hooks.IMoodleClientHook> _hooks;

    public IUserService Users { get; }
    public ISystemService System { get; }
    public IEnrolmentService Enrolments { get; }
    public ICourseService Courses { get; }
    public IGroupService Groups { get; }
    public IGradeService Grades { get; }
    public ICalendarService Calendar { get; }
    public ICustomFunctionService Custom { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MoodleClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client to use for requests.</param>
    /// <param name="options">The SDK configuration options.</param>
    /// <param name="hooks">A collection of hooks to intercept requests and responses.</param>
    public MoodleClient(HttpClient httpClient, MoodleOptions options, IEnumerable<MoodleSdk.Hooks.IMoodleClientHook> hooks)
    {
        _httpClient = httpClient;
        _options = options;
        _hooks = hooks;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        Users = new UserService(this);
        System = new SystemService(this);
        Enrolments = new EnrolmentService(this);
        Courses = new CourseService(this);
        Groups = new GroupService(this);
        Grades = new GradeService(this);
        Calendar = new CalendarService(this);
        Custom = new CustomFunctionService(this);
    }

    public async Task<MoodleResult<T>> ExecuteAsync<T>(MoodleRequest request)
    {
        try
        {
            foreach (var hook in _hooks) await hook.OnBeforeRequestAsync(request);

            var token = request.OverrideToken ?? _options.DefaultToken;
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("Moodle token is not set. Provide a default token in MoodleOptions or an override in the request.");
            }

            var uriBuilder = new UriBuilder(_options.BaseUrl);
            if (!uriBuilder.Path.EndsWith("/")) uriBuilder.Path += "/";
            uriBuilder.Path += "webservice/rest/server.php";

            var queryParams = new List<KeyValuePair<string, string>>
            {
                new("wstoken", token),
                new("wsfunction", request.Function),
                new("moodlewsrestformat", _options.Format.ToString().ToLowerInvariant())
            };

            foreach (var param in request.Parameters)
            {
                AddParameters(queryParams, param.Key, param.Value);
            }

            HttpRequestMessage message;
            if (request.Method == HttpMethod.Get)
            {
                var query = string.Join("&", queryParams.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
                uriBuilder.Query = query;
                message = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
            }
            else
            {
                message = new HttpRequestMessage(request.Method, uriBuilder.Uri)
                {
                    Content = new FormUrlEncodedContent(queryParams)
                };
            }

            var response = await _httpClient.SendAsync(message);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            foreach (var hook in _hooks) await hook.OnAfterResponseAsync(request, content);

            return ParseResponse<T>(content);
        }
        catch (Exception ex)
        {
            foreach (var hook in _hooks) await hook.OnErrorAsync(request, ex);
            throw;
        }
    }

    private void AddParameters(List<KeyValuePair<string, string>> queryParams, string key, object value)
    {
        // Moodle expects parameters in a specific format for arrays and objects:
        // key[0]=val1&key[1]=val2
        // user[username]=admin&user[password]=123

        if (value is System.Collections.IEnumerable enumerable && value is not string)
        {
            int i = 0;
            foreach (var item in enumerable)
            {
                AddParameters(queryParams, $"{key}[{i}]", item);
                i++;
            }
        }
        else if (value is IDictionary<string, object> dict)
        {
            foreach (var kvp in dict)
            {
                AddParameters(queryParams, $"{key}[{kvp.Key}]", kvp.Value);
            }
        }
        else
        {
            queryParams.Add(new KeyValuePair<string, string>(key, value?.ToString() ?? string.Empty));
        }
    }

    private MoodleResult<T> ParseResponse<T>(string content)
    {
        // Check for Moodle error format first
        if (content.Contains("\"exception\":") || content.Contains("\"errorcode\":"))
        {
            var error = JsonSerializer.Deserialize<MoodleError>(content, _jsonOptions);
            if (error != null && (!string.IsNullOrEmpty(error.ErrorCode) || !string.IsNullOrEmpty(error.Exception)))
            {
                return MoodleResult<T>.Failure(error);
            }
        }

        // Moodle sometimes returns warnings and data together.
        // We need to handle this. For now, let's assume T matches the structure.
        // If T is an array, Moodle returns [item1, item2]
        // If T is an object, Moodle returns { ... }

        try
        {
            var data = JsonSerializer.Deserialize<T>(content, _jsonOptions);
            return MoodleResult<T>.Success(data!);
        }
        catch (JsonException ex)
        {
            // Fallback: maybe it's a single item wrap?
            // Actually, Moodle's JSON is usually flat or nested as requested.
            throw new MoodleApiException($"Failed to deserialize Moodle response: {ex.Message}", new MoodleError { Message = content });
        }
    }

    public void Dispose()
    {
        // HttpClient is usually managed by the caller in modern .NET
    }
}
