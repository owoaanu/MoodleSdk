using MoodleApi.Models;
using MoodleSdk.Core;

namespace MoodleSdk.Services;

/// <summary>
/// Provides access to calendar and event functions in the Moodle API.
/// </summary>
public interface ICalendarService
{
    Task<MoodleResult<ListOfEvents>> GetCalendarEventsAsync(int[]? groupIds = null, int[]? courseIds = null, int[]? eventIds = null);
    Task<MoodleResult<ListOfEvents>> CreateCalendarEventsAsync(IEnumerable<IDictionary<string, object>> events);
    Task<MoodleResult<ListOfEvents>> DeleteCalendarEventsAsync(IEnumerable<IDictionary<string, object>> events);
}
