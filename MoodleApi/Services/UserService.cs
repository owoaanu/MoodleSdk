using MoodleApi.Models;
using MoodleSdk.Core;
using MoodleSdk.Models;

namespace MoodleSdk.Services;

/// <summary>
/// Implementation of the IUserService providing access to Moodle user functions.
/// </summary>
public sealed class UserService : IUserService
{
    private readonly IMoodleClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class.
    /// </summary>
    /// <param name="client">The Moodle client.</param>
    public UserService(IMoodleClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Search for users matching a set of criteria.
    /// </summary>
    /// <param name="criteriaKey">The first criteria key (e.g. 'username').</param>
    /// <param name="criteriaValue">The first criteria value.</param>
    /// <param name="additionalCriteria">Optional additional criteria pairs.</param>
    /// <returns>A list of matching users.</returns>
    public Task<MoodleResult<UsersData>> GetUsersAsync(string criteriaKey, string criteriaValue, params (string Key, string Value)[] additionalCriteria)
    {
        var parameters = new Dictionary<string, object>
        {
            { "criteria[0][key]", criteriaKey },
            { "criteria[0][value]", criteriaValue }
        };

        for (int i = 0; i < additionalCriteria.Length; i++)
        {
            parameters.Add($"criteria[{i + 1}][key]", additionalCriteria[i].Key);
            parameters.Add($"criteria[{i + 1}][value]", additionalCriteria[i].Value);
        }

        return _client.ExecuteAsync<UsersData>(new MoodleRequest
        {
            Function = MoodleFunctions.User.GetUsers,
            Parameters = parameters
        });
    }

    /// <summary>
    /// Get users by a specific field and values.
    /// </summary>
    /// <param name="field">The field to search by (e.g. 'id', 'username').</param>
    /// <param name="values">The values to match.</param>
    /// <returns>A list of matching users.</returns>
    public Task<MoodleResult<User[]>> GetUsersByFieldAsync(string field, params string[] values)
    {
        var parameters = new Dictionary<string, object>
        {
            { "field", field }
        };

        for (int i = 0; i < values.Length; i++)
        {
            parameters.Add($"values[{i}]", values[i]);
        }

        return _client.ExecuteAsync<User[]>(new MoodleRequest
        {
            Function = MoodleFunctions.User.GetUsersByField,
            Parameters = parameters
        });
    }

    /// <summary>
    /// Create a new user in Moodle.
    /// </summary>
    /// <param name="user">The user data to create.</param>
    /// <returns>The newly created user information.</returns>
    public Task<MoodleResult<NewUser[]>> CreateUserAsync(CreateUserRequest user)
    {
        var parameters = new Dictionary<string, object>
        {
            { "users[0][username]", user.UserName },
            { "users[0][password]", user.Password },
            { "users[0][firstname]", user.FirstName },
            { "users[0][lastname]", user.LastName },
            { "users[0][email]", user.Email }
        };

        if (!string.IsNullOrEmpty(user.Auth)) parameters.Add("users[0][auth]", user.Auth);
        if (!string.IsNullOrEmpty(user.IdNumber)) parameters.Add("users[0][idnumber]", user.IdNumber);
        if (!string.IsNullOrEmpty(user.Language)) parameters.Add("users[0][lang]", user.Language);
        if (!string.IsNullOrEmpty(user.Theme)) parameters.Add("users[0][theme]", user.Theme);
        if (!string.IsNullOrEmpty(user.Timezone)) parameters.Add("users[0][timezone]", user.Timezone);
        if (!string.IsNullOrEmpty(user.Description)) parameters.Add("users[0][description]", user.Description);
        if (!string.IsNullOrEmpty(user.City)) parameters.Add("users[0][city]", user.City);
        if (!string.IsNullOrEmpty(user.Country)) parameters.Add("users[0][country]", user.Country);
        if (!string.IsNullOrEmpty(user.CalendarType)) parameters.Add("users[0][calendartype]", user.CalendarType);
        if (!string.IsNullOrEmpty(user.MailFormat)) parameters.Add("users[0][mailformat]", user.MailFormat);
        if (!string.IsNullOrEmpty(user.FirstNamePhonetic)) parameters.Add("users[0][firstnamephonetic]", user.FirstNamePhonetic);
        if (!string.IsNullOrEmpty(user.LastNamePhonetic)) parameters.Add("users[0][lastnamephonetic]", user.LastNamePhonetic);
        if (!string.IsNullOrEmpty(user.MiddleName)) parameters.Add("users[0][middlename]", user.MiddleName);
        if (!string.IsNullOrEmpty(user.AlternateName)) parameters.Add("users[0][alternatename]", user.AlternateName);

        if (user.Preferences != null)
        {
            for (int i = 0; i < user.Preferences.Count; i++)
            {
                parameters.Add($"users[0][preferences][{i}][type]", user.Preferences[i].Name);
                parameters.Add($"users[0][preferences][{i}][value]", user.Preferences[i].Value);
            }
        }

        if (user.CustomFields != null)
        {
            for (int i = 0; i < user.CustomFields.Count; i++)
            {
                parameters.Add($"users[0][customfields][{i}][type]", user.CustomFields[i].Type);
                parameters.Add($"users[0][customfields][{i}][value]", user.CustomFields[i].Value);
            }
        }

        return _client.ExecuteAsync<NewUser[]>(new MoodleRequest
        {
            Function = MoodleFunctions.User.CreateUsers,
            Method = HttpMethod.Post,
            Parameters = parameters
        });
    }

    /// <summary>
    /// Update an existing user in Moodle.
    /// </summary>
    /// <param name="id">The Moodle ID of the user to update.</param>
    /// <param name="user">The updated user data.</param>
    /// <returns>Success indicator.</returns>
    public Task<MoodleResult<Success>> UpdateUserAsync(int id, CreateUserRequest user)
    {
        var parameters = new Dictionary<string, object>
        {
            { "users[0][id]", id }
        };

        if (!string.IsNullOrEmpty(user.UserName)) parameters.Add("users[0][username]", user.UserName);
        if (!string.IsNullOrEmpty(user.Password)) parameters.Add("users[0][password]", user.Password);
        if (!string.IsNullOrEmpty(user.FirstName)) parameters.Add("users[0][firstname]", user.FirstName);
        if (!string.IsNullOrEmpty(user.LastName)) parameters.Add("users[0][lastname]", user.LastName);
        if (!string.IsNullOrEmpty(user.Email)) parameters.Add("users[0][email]", user.Email);
        if (!string.IsNullOrEmpty(user.Auth)) parameters.Add("users[0][auth]", user.Auth);
        if (!string.IsNullOrEmpty(user.IdNumber)) parameters.Add("users[0][idnumber]", user.IdNumber);
        if (!string.IsNullOrEmpty(user.Language)) parameters.Add("users[0][lang]", user.Language);
        if (!string.IsNullOrEmpty(user.Theme)) parameters.Add("users[0][theme]", user.Theme);
        if (!string.IsNullOrEmpty(user.Timezone)) parameters.Add("users[0][timezone]", user.Timezone);
        if (!string.IsNullOrEmpty(user.Description)) parameters.Add("users[0][description]", user.Description);
        if (!string.IsNullOrEmpty(user.City)) parameters.Add("users[0][city]", user.City);
        if (!string.IsNullOrEmpty(user.Country)) parameters.Add("users[0][country]", user.Country);
        if (!string.IsNullOrEmpty(user.CalendarType)) parameters.Add("users[0][calendartype]", user.CalendarType);
        if (!string.IsNullOrEmpty(user.MailFormat)) parameters.Add("users[0][mailformat]", user.MailFormat);
        if (!string.IsNullOrEmpty(user.FirstNamePhonetic)) parameters.Add("users[0][firstnamephonetic]", user.FirstNamePhonetic);
        if (!string.IsNullOrEmpty(user.LastNamePhonetic)) parameters.Add("users[0][lastnamephonetic]", user.LastNamePhonetic);
        if (!string.IsNullOrEmpty(user.MiddleName)) parameters.Add("users[0][middlename]", user.MiddleName);
        if (!string.IsNullOrEmpty(user.AlternateName)) parameters.Add("users[0][alternatename]", user.AlternateName);

        if (user.Preferences != null)
        {
            for (int i = 0; i < user.Preferences.Count; i++)
            {
                parameters.Add($"users[0][preferences][{i}][type]", user.Preferences[i].Name);
                parameters.Add($"users[0][preferences][{i}][value]", user.Preferences[i].Value);
            }
        }

        if (user.CustomFields != null)
        {
            for (int i = 0; i < user.CustomFields.Count; i++)
            {
                parameters.Add($"users[0][customfields][{i}][type]", user.CustomFields[i].Type);
                parameters.Add($"users[0][customfields][{i}][value]", user.CustomFields[i].Value);
            }
        }

        return _client.ExecuteAsync<Success>(new MoodleRequest
        {
            Function = MoodleFunctions.User.UpdateUsers,
            Method = HttpMethod.Post,
            Parameters = parameters
        });
    }

    /// <summary>
    /// Delete one or more users from Moodle.
    /// </summary>
    /// <param name="userIds">The IDs of the users to delete.</param>
    /// <returns>Success indicator.</returns>
    public Task<MoodleResult<Success>> DeleteUsersAsync(params int[] userIds)
    {
        var parameters = new Dictionary<string, object>();
        for (int i = 0; i < userIds.Length; i++)
        {
            parameters.Add($"userids[{i}]", userIds[i]);
        }

        return _client.ExecuteAsync<Success>(new MoodleRequest
        {
            Function = MoodleFunctions.User.DeleteUsers,
            Method = HttpMethod.Post,
            Parameters = parameters
        });
    }
}
