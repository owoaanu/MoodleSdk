# Getting Started with Moodle SDK

The `MoodleSdk` provides two primary ways to interact with the Moodle API: using **Dependency Injection (DI)** (recommended for ASP.NET Core) or **Direct Initialization**.

## Installation

Install the package via NuGet:

```bash
dotnet add package MoodleSdk
```

---

## 1. Dependency Injection (Recommended)

This approach is best for modern applications like ASP.NET Core.

### Configuration
Update your `appsettings.json`:

```json
{
  "Moodle": {
    "BaseUrl": "https://your-moodle-site.com",
    "DefaultToken": "your-access-token"
  }
}
```

### Service Registration
In your `Program.cs` or `Startup.cs`:

```csharp
using MoodleSdk.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register Moodle SDK
builder.Services.AddMoodleSdk(options => {
    options.BaseUrl = builder.Configuration["Moodle:BaseUrl"];
    options.DefaultToken = builder.Configuration["Moodle:DefaultToken"];
});
```

### Usage
Inject `IMoodleClient` into your services or controllers:

```csharp
public class MyService
{
    private readonly IMoodleClient _moodle;

    public MyService(IMoodleClient moodle)
    {
        _moodle = moodle;
    }

    public async Task GetSiteData()
    {
        var result = await _moodle.System.GetSiteInfoAsync();
        if (result.IsSuccess)
        {
            Console.WriteLine($"Site Name: {result.Data.SiteName}");
        }
    }
}
```

---

## 2. Direct Initialization

Use this in simple console apps or whenever you don't use DI.

```csharp
using MoodleSdk;
using MoodleSdk.Core;

// 1. Setup options
var options = new MoodleOptions
{
    BaseUrl = "https://your-moodle-site.com",
    DefaultToken = "your-access-token"
};

// 2. Initialize HttpClient (Managed by you)
using var httpClient = new HttpClient();

// 3. Create the client
IMoodleClient moodle = new MoodleClient(httpClient, options, Enumerable.Empty<IMoodleClientHook>());

// 4. Start using services
var courses = await moodle.Courses.GetCoursesAsync();
```

---

## Service Hierarchy

The `IMoodleClient` organizes functions into logical domains:

- `moodle.Users`: Create, update, delete, and search users.
- `moodle.Courses`: Manage courses and categories.
- `moodle.Enrolments`: Handle user enrolments and roles.
- `moodle.Groups`: Manage groups and group memberships.
- `moodle.Grades`: Retrieve grade data.
- `moodle.Calendar`: Manage events.
- `moodle.System`: General site information.
- `moodle.Custom`: Invoke any arbitrary Moodle function.
