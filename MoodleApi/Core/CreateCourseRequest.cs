namespace MoodleSdk.Models;

/// <summary>
/// Data required to create or update a course in Moodle.
/// </summary>
public sealed class CreateCourseRequest
{
    public string FullName { get; set; } = null!;
    public string ShortName { get; set; } = null!;
    public int CategoryId { get; set; }
    public string? IdNumber { get; set; }
    public string? Summary { get; set; }
    public int? SummaryFormat { get; set; }
    public string? Format { get; set; }
    public int? ShowGrades { get; set; }
    public int? NewsItems { get; set; }
    public int? StartDate { get; set; }
    public int? EndDate { get; set; }
    public int? NumSections { get; set; }
    public int? MaxBytes { get; set; }
    public int? ShowReports { get; set; }
    public int? Visible { get; set; }
    public int? GroupMode { get; set; }
    public int? GroupModeForce { get; set; }
    public int? DefaultGroupId { get; set; }
}
