using Microsoft.Extensions.DependencyInjection;
using MoodleSdk;
using MoodleSdk.Core;
using MoodleSdk.Services;
using MoodleSdk.Hooks;
using MoodleApi;

namespace MoodleSdk.Extensions
{
    public static class MoodleApiServiceCollectionExtensions
    {
        public static IServiceCollection AddMoodleSdk(this IServiceCollection services, Action<MoodleOptions> configure)
        {
            var options = new MoodleOptions();
            configure(options);
            services.AddSingleton(options);
            services.AddScoped<IMoodleClient, MoodleClient>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISystemService, SystemService>();
            services.AddScoped<IEnrolmentService, EnrolmentService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IGradeService, GradeService>();
            services.AddScoped<ICalendarService, CalendarService>();
            services.AddScoped<ICustomFunctionService, CustomFunctionService>();
            return services;
        }

        public static IServiceCollection AddMoodleHook<THook>(this IServiceCollection services)
            where THook : class, IMoodleClientHook
        {
            services.AddScoped<IMoodleClientHook, THook>();
            return services;
        }

        public static IServiceCollection AddMoodleLogging(this IServiceCollection services)
        {
            return services.AddMoodleHook<LoggingHook>();
        }

        public static IServiceCollection AddMoodleApi(this IServiceCollection services)
        {
            services.AddScoped<Moodle>();
            return services;
        }
    }
}
