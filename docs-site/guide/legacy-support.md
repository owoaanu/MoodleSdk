# Legacy Support

If you are upgrading from an older version of this library that used the `Moodle` class, your code will continue to work.

## Backward Compatibility
The `Moodle` class has been refactored into a compatibility façade. It implements the same old methods but internally delegates the work to the new `IMoodleClient` architecture.

```csharp
// Still works!
var moodle = new Moodle(url, token);
var response = await moodle.GetCourses();
```

## Recommendation
The `Moodle` class is now marked as `[Obsolete]`. We recommend migrating to the new `IMoodleClient` and service-based architecture for:
1. Better support for Dependency Injection.
2. Improved error handling and specialized exceptions.
3. Access to new features like the Hook system.
4. Better performance and modern .NET standards.
