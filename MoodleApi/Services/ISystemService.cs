using MoodleApi.Models;
using MoodleSdk.Core;

namespace MoodleSdk.Services;

public interface ISystemService
{
    Task<MoodleResult<SiteInfo>> GetSiteInfoAsync(string serviceShortName = "");
}
