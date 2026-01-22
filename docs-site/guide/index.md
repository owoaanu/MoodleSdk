# Moodle SDK .NET

[![NuGet Version](https://img.shields.io/nuget/v/MoodleSdk.svg)](https://www.nuget.org/packages/MoodleSdk)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A modern, high-performance .NET 8.0 SDK for the Moodle Web Services API.

This library simplifies integrating Moodle with .NET applications by providing a strongly-typed, service-oriented client with native support for Dependency Injection and modern C# features.

---

## Key Features

- **🚀 Modern .NET 8.0**: Built for performance and modern cloud applications.
- **💉 Dependency Injection**: First-class support for `IServiceCollection`.
- **🛡️ Strongly Typed**: No more manual dictionary building; use models for Users, Courses, Groups, and more.
- **🎣 Extensible (Hooks)**: Intercept requests, log responses, or add custom validation with a native Hook system.
- **🧩 Custom Functions**: Easily call any Moodle API function that isn't yet in the strongly-typed service.
- **⚠️ Specialized Exceptions**: Moodle error codes are automatically mapped to granular exceptions like `MoodlePermissionException`.
- **🔄 Backward Compatible**: Includes a compatibility façade for existing `Moodle.cs` users.

---

## Quick Start

### 1. Install via NuGet
```bash
dotnet add package MoodleSdk
```

### 2. Register Services
```csharp
builder.Services.AddMoodleSdk(options => {
    options.BaseUrl = "https://your-moodle.com";
    options.DefaultToken = "your-token";
});
```

### 3. Use the Client
```csharp
public class CourseApp(IMoodleClient moodle)
{
    public async Task Run()
    {
        var result = await moodle.Courses.GetCoursesAsync();
        if (result.IsSuccess)
        {
            foreach(var c in result.Data) Console.WriteLine(c.FullName);
        }
    }
}
```

---

## Detailed Documentation

- [**Moodle Setup Guide**](./moodle-setup.md): How to enable Web Services and get your API Token.
- [**Getting Started**](./getting-started.md): Detailed guide for both DI and Non-DI applications.
- [**Advanced Features**](./advanced-features.md): Learn about the Hook system, Custom Functions, and Error Handling.
- [**Migration & Legacy Support**](./legacy-support.md): Information for users upgrading from older versions.

---

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request or open an Issue on [GitHub](https://github.com/owoaanu/MoodleSdk).

## License

This project is licensed under the **MIT License**. See the [LICENSE](./LICENSE) file for details.

## Authors

- **Owoaanu**
- **Cyrus.Sushiant**
- **SmartClouds**