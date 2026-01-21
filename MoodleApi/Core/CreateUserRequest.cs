namespace MoodleSdk.Models;

/// <summary>
/// Data required to create a new user in Moodle.
/// </summary>
public sealed class CreateUserRequest
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Auth { get; set; }
    public string? IdNumber { get; set; }
    public string? Language { get; set; }
    public string? Theme { get; set; }
    public string? Timezone { get; set; }
    public string? Description { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? CalendarType { get; set; }
    public string? MailFormat { get; set; }
    public string? FirstNamePhonetic { get; set; }
    public string? LastNamePhonetic { get; set; }
    public string? MiddleName { get; set; }
    public string? AlternateName { get; set; }
    public List<(string Name, string Value)>? Preferences { get; set; }
    public List<(string Type, string Value)>? CustomFields { get; set; }
}
