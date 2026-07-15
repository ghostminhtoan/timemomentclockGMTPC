using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Time_Moment_Clock_GMTPC.Pages;
using Time_Moment_Clock_GMTPC.Services;
using Time_Moment_Clock_GMTPC.ViewModels;

namespace Time_Moment_Clock_GMTPC
{
    public static class MauiProgram
    {
        public static IServiceProvider Services { get; private set; } = default!;

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<IAlertService, AlertService>();
#if WINDOWS
            builder.Services.AddSingleton<IWindowManagerService, Platforms.Windows.WindowManagerService>();
            builder.Services.AddSingleton<IAlarmSchedulerService, FallbackAlarmSchedulerService>();
#elif ANDROID
            builder.Services.AddSingleton<IWindowManagerService, Platforms.Android.WindowManagerService>();
            builder.Services.AddSingleton<IAlarmSchedulerService, Platforms.Android.AlarmSchedulerService>();
#else
            builder.Services.AddSingleton<IWindowManagerService, FallbackWindowManagerService>();
            builder.Services.AddSingleton<IAlarmSchedulerService, FallbackAlarmSchedulerService>();
#endif
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddTransient<ClockPage>();
            builder.Services.AddTransient<StopwatchPage>();
            builder.Services.AddTransient<CountdownPage>();
            builder.Services.AddTransient<AlarmPage>();
            builder.Services.AddTransient<ClockViewModel>();
            builder.Services.AddTransient<StopwatchViewModel>();
            builder.Services.AddTransient<CountdownViewModel>();
            builder.Services.AddTransient<AlarmViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();
            Services = app.Services;
            return app;
        }
    }
}
