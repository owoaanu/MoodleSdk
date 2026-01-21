using MoodleApi.Models;
using MoodleSdk.Core;
using MoodleSdk.Models;

namespace MoodleSdk.Services;

/// <summary>
/// Service for managing users in Moodle.
/// </summary>
/// <summary>
/// Provides access to user-related functions in the Moodle API.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Search for users matching criteria.
    /// </summary>
    Task<MoodleResult<UsersData>> GetUsersAsync(string criteriaKey, string criteriaValue, params (string Key, string Value)[] additionalCriteria);

    /// <summary>
    /// Retrieve users information for a specified unique field.
    /// </summary>
    Task<MoodleResult<User[]>> GetUsersByFieldAsync(string field, params string[] values);

    /// <summary>
    /// Create a new user.
    /// </summary>
    Task<MoodleResult<NewUser[]>> CreateUserAsync(CreateUserRequest user);

    /// <summary>
    /// Update an existing user.
    /// </summary>
    Task<MoodleResult<Success>> UpdateUserAsync(int id, CreateUserRequest user);

    /// <summary>
    /// Delete users.
    /// </summary>
    Task<MoodleResult<Success>> DeleteUsersAsync(params int[] userIds);
}
