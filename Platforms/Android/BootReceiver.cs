using Android.App;
using Android.Content;
using Microsoft.Maui.Storage;
using System.Text.Json;
using Time_Moment_Clock_GMTPC.Models;

namespace Time_Moment_Clock_GMTPC.Platforms.Android
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted, Intent.ActionLockedBootCompleted, Intent.ActionMyPackageReplaced })]
    public class BootReceiver : BroadcastReceiver
    {
        private const string AlarmStorageKey = "saved_alarms";

        public override void OnReceive(Context? context, Intent? intent)
        {
            if (context == null)
            {
                return;
            }

            var raw = Preferences.Default.Get(AlarmStorageKey, string.Empty);
            if (string.IsNullOrWhiteSpace(raw))
            {
                return;
            }

            var alarms = JsonSerializer.Deserialize<List<AlarmItem>>(raw);
            if (alarms == null)
            {
                return;
            }

            AlarmPlatformScheduler.EnsureAlarmChannel(context);
            foreach (var alarm in alarms)
            {
                if (alarm.IsEnabled)
                {
                    AlarmPlatformScheduler.ScheduleAlarm(context, alarm);
                }
            }
        }
    }
}
