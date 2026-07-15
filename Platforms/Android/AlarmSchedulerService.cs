using Android.App;
using Android.Content;
using Android.Provider;
using Microsoft.Maui.ApplicationModel;
using Time_Moment_Clock_GMTPC.Models;
using Time_Moment_Clock_GMTPC.Services;

namespace Time_Moment_Clock_GMTPC.Platforms.Android
{
    public class AlarmSchedulerService : IAlarmSchedulerService
    {
        public bool SupportsBackgroundScheduling => true;

        public async Task EnsurePermissionsAsync()
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                await Permissions.RequestAsync<Permissions.PostNotifications>();
            }

            var activity = Platform.CurrentActivity;
            if (activity == null || !OperatingSystem.IsAndroidVersionAtLeast(31))
            {
                return;
            }

            var alarmManager = (AlarmManager?)activity.GetSystemService(Context.AlarmService);
            if (alarmManager?.CanScheduleExactAlarms() == true)
            {
                return;
            }

            var intent = new Intent(Settings.ActionRequestScheduleExactAlarm);
            intent.SetData(global::Android.Net.Uri.Parse($"package:{activity.PackageName}"));
            intent.AddFlags(ActivityFlags.NewTask);
            activity.StartActivity(intent);
        }

        public Task SyncAsync(IEnumerable<AlarmItem> alarms)
        {
            var context = Platform.AppContext;
            AlarmPlatformScheduler.EnsureAlarmChannel(context);

            foreach (var alarm in alarms)
            {
                if (alarm.IsEnabled)
                {
                    AlarmPlatformScheduler.ScheduleAlarm(context, alarm);
                }
                else
                {
                    AlarmPlatformScheduler.CancelAlarm(context, alarm.Id);
                }
            }

            return Task.CompletedTask;
        }
    }
}
