using MoodleApi.Models;
using MoodleSdk.Core;

namespace MoodleSdk.Services;

/// <summary>
/// Implementation of the ICalendarService providing access to Moodle calendar functions.
/// </summary>
public sealed class CalendarService : ICalendarService
{
    private readonly IMoodleClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="CalendarService"/> class.
    /// </summary>
    /// <param name="client">The Moodle client.</param>
    public CalendarService(IMoodleClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Get calendar events based on group, course, or event IDs.
    /// </summary>
    /// <param name="groupIds">Optional array of group IDs.</param>
    /// <param name="courseIds">Optional array of course IDs.</param>
    /// <param name="eventIds">Optional array of event IDs.</param>
    /// <returns>A list of calendar events.</returns>
    public Task<MoodleResult<ListOfEvents>> GetCalendarEventsAsync(int[]? groupIds = null, int[]? courseIds = null, int[]? eventIds = null)
    {
        var parameters = new Dictionary<string, object>();
        if (groupIds != null)
        {
            for (int i = 0; i < groupIds.Length; i++) parameters.Add($"events[groupids][{i}]", groupIds[i]);
        }
        if (courseIds != null)
        {
            for (int i = 0; i < courseIds.Length; i++) parameters.Add($"events[courseids][{i}]", courseIds[i]);
        }
        if (eventIds != null)
        {
            for (int i = 0; i < eventIds.Length; i++) parameters.Add($"events[eventids][{i}]", eventIds[i]);
        }

        return _client.ExecuteAsync<ListOfEvents>(new MoodleRequest
        {
            Function = MoodleFunctions.Calendar.GetCalendarEvents,
            Parameters = parameters
        });
    }

    /// <summary>
    /// Create new calendar events.
    /// </summary>
    /// <param name="events">A list of event property dictionaries.</param>
    /// <returns>The newly created events.</returns>
    public Task<MoodleResult<ListOfEvents>> CreateCalendarEventsAsync(IEnumerable<IDictionary<string, object>> events)
    {
        var parameters = new Dictionary<string, object>();
        int i = 0;
        foreach (var evt in events)
        {
            foreach (var kvp in evt)
            {
                parameters.Add($"events[{i}][{kvp.Key}]", kvp.Value);
            }
            i++;
        }

        return _client.ExecuteAsync<ListOfEvents>(new MoodleRequest
        {
            Function = MoodleFunctions.Calendar.CreateCalendarEvents,
            Method = HttpMethod.Post,
            Parameters = parameters
        });
    }

    /// <summary>
    /// Delete calendar events.
    /// </summary>
    /// <param name="events">A list of event property dictionaries (containing event IDs).</param>
    /// <returns>The result of the deletion.</returns>
    public Task<MoodleResult<ListOfEvents>> DeleteCalendarEventsAsync(IEnumerable<IDictionary<string, object>> events)
    {
        var parameters = new Dictionary<string, object>();
        int i = 0;
        foreach (var evt in events)
        {
            foreach (var kvp in evt)
            {
                parameters.Add($"events[{i}][{kvp.Key}]", kvp.Value);
            }
            i++;
        }

        return _client.ExecuteAsync<ListOfEvents>(new MoodleRequest
        {
            Function = MoodleFunctions.Calendar.DeleteCalendarEvents,
            Method = HttpMethod.Post,
            Parameters = parameters
        });
    }
}
