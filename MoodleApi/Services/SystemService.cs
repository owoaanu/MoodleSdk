using MoodleApi.Models;
using MoodleSdk.Core;

namespace MoodleSdk.Services;

/// <summary>
/// Implementation of the ISystemService providing access to general Moodle site information.
/// </summary>
public sealed class SystemService : ISystemService
{
    private readonly IMoodleClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemService"/> class.
    /// </summary>
    /// <param name="client">The Moodle client.</param>
    public SystemService(IMoodleClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Returns general information about the site.
    /// </summary>
    /// <param name="serviceShortName">Optional service short name to filter information.</param>
    /// <returns>Site information including version, site name, and user details.</returns>
    public Task<MoodleResult<SiteInfo>> GetSiteInfoAsync(string serviceShortName = "")
    {
        var parameters = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(serviceShortName))
        {
            parameters.Add("serviceshortnames[0]", serviceShortName);
        }

        return _client.ExecuteAsync<SiteInfo>(new MoodleRequest
        {
            Function = MoodleFunctions.System.GetSiteInfo,
            Parameters = parameters
        });
    }
}
