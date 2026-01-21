using MoodleSdk.Core;

namespace MoodleSdk.Services;

public sealed class CustomFunctionService : ICustomFunctionService
{
    private readonly IMoodleClient _client;

    public CustomFunctionService(IMoodleClient client)
    {
        _client = client;
    }

    public Task<MoodleResult<T>> CallAsync<T>(string functionName, HttpMethod method, IDictionary<string, object> parameters) where T : class
    {
        return _client.ExecuteAsync<T>(new MoodleRequest
        {
            Function = functionName,
            Method = method,
            Parameters = parameters
        });
    }
}
