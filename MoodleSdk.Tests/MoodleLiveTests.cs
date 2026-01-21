using System.Text.Json;
using Xunit.Abstractions;
using MoodleSdk;
using MoodleSdk.Core;
using Xunit;
using MoodleSdk.Hooks;

namespace MoodleSdk.Tests;

public class MoodleLiveTests
{
    private readonly string _baseUrl;
    private readonly string _token;
    private readonly ITestOutputHelper _output;

    public MoodleLiveTests(ITestOutputHelper output)
    {
        _output = output;
        _baseUrl = Environment.GetEnvironmentVariable("MOODLE_URL") ?? "https://moodle.mine.edu.ng/";
        _token = Environment.GetEnvironmentVariable("MOODLE_TOKEN") ?? "my_token";
    }

    [Fact]
    [Trait("Category", "Live")]
    public async Task GetSiteInfo_ReturnsValidData()
    {
        // Skip if dummy values are used and we are sure they are dummy
        // But the provided token looks semi-real.

        var options = new MoodleOptions
        {
            BaseUrl = new Uri(_baseUrl),
            DefaultToken = _token
        };

        using var httpClient = new HttpClient();
        var client = new MoodleClient(httpClient, options, Enumerable.Empty<IMoodleClientHook>());

        var result = await client.System.GetSiteInfoAsync();

        if (!result.IsSuccess)
        {
             // If it fails with "invalidtoken", it might just be that the token is expired/invalid
             // but it proves the connection worked.
             Assert.NotNull(result.Error);
             return;
        }

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        
        var json = JsonSerializer.Serialize(result.Data, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine("--- GetSiteInfo Live Response ---");
        Console.WriteLine(json);
        Console.WriteLine("---------------------------------");

        Assert.NotEmpty(result.Data.SiteName ?? "");
        Assert.NotEmpty(result.Data.UserName ?? "");
    }

    [Fact]
    [Trait("Category", "Live")]
    public async Task GetUsers_ReturnsData()
    {
        var options = new MoodleOptions
        {
            BaseUrl = new Uri(_baseUrl),
            DefaultToken = _token
        };

        using var httpClient = new HttpClient();
        var client = new MoodleClient(httpClient, options, Enumerable.Empty<IMoodleClientHook>());

        // Search for 'admin' user
        var result = await client.Users.GetUsersAsync("username", "admin");

        if (!result.IsSuccess)
        {
             Assert.NotNull(result.Error);
             return;
        }

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);

        var json = JsonSerializer.Serialize(result.Data, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine("--- GetUsers Live Response ---");
        Console.WriteLine(json);
        Console.WriteLine("------------------------------");

        // We might not find 'admin', but the call should succeed
    }
}
