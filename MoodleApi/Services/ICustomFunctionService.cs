using MoodleSdk.Core;

namespace MoodleSdk.Services;

/// <summary>
/// Provides access to call any Moodle web service function, including those from custom plugins.
/// </summary>
public interface ICustomFunctionService
{
    Task<MoodleResult<T>> CallAsync<T>(string functionName, HttpMethod method, IDictionary<string, object> parameters) where T : class;
}
