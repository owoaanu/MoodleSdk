using System.Text.Json.Serialization;
using MoodleSdk.Extensions;

namespace MoodleApi.Models;

public class CourseFormatOption
{
    public string Name { get; set; } = string.Empty;

    [JsonConverter(typeof(FlexibleStringConverter))]
    public string Value { get; set; } = string.Empty;
}