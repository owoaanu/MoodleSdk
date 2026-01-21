using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoodleSdk.Extensions;
using MoodleSdk;
using MoodleSdk.Core;
using System.Text.Json;

// Setup generic host for DI and HttpClient
var builder = new HostBuilder().ConfigureServices((hostContext, services) =>
{
    services.AddHttpClient();

    // Register MoodleSdk with configuration
    services.AddMoodleSdk(options =>
    {
        options.BaseUrl = new Uri("https://yourlivelms.edu/");
        options.DefaultToken = "your_token_here";
        options.Format = MoodleApi.Models.MoodleFormat.JSON;
    });
}).UseConsoleLifetime();

var host = builder.Build();

using var serviceScope = host.Services.CreateScope();
var services = serviceScope.ServiceProvider;

// Use the new IMoodleClient
var moodleClient = services.GetRequiredService<IMoodleClient>();

var jsonOptions = new JsonSerializerOptions { WriteIndented = true };

try
{
    Console.WriteLine("--- MoodleSdk Live Integration Example ---");
    Console.WriteLine("Fetching site info...");
    var siteInfoResult = await moodleClient.System.GetSiteInfoAsync();

    if (siteInfoResult.IsSuccess)
    {
        Console.WriteLine("Site Info JSON:");
        // Console.WriteLine(JsonSerializer.Serialize(siteInfoResult.Data, jsonOptions));

        Console.WriteLine("\nSearching for user 'admin'...");
        var userResult = await moodleClient.Users.GetUsersAsync("username", "adebayo-adesina");

        if (userResult.IsSuccess)
        {
            Console.WriteLine("User Search JSON:");
            Console.WriteLine(JsonSerializer.Serialize(userResult.Data, jsonOptions));
        }
        else
        {
            Console.WriteLine($"User search failed: {userResult.Error?.Message}");
        }

        var courseResult = await moodleClient.Courses.GetCoursesByFieldAsync("shortname", "CSC 211");

        if (courseResult.IsSuccess)
        {
            Console.WriteLine("Course Search JSON:");
            Console.WriteLine(JsonSerializer.Serialize(courseResult.Data, jsonOptions));
        }
        else
        {
            Console.WriteLine($"Course search failed: {courseResult.Error?.Message}");
        }
    }
    else
    {
        Console.WriteLine($"API Call Failed: {siteInfoResult.Error?.Message}");
        Console.WriteLine("Error Details JSON:");
        Console.WriteLine(JsonSerializer.Serialize(siteInfoResult.Error, jsonOptions));
    }
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred during execution: {ex.Message}");
}

Console.WriteLine("\nExecution finished.");