using Android.App;
using Android.Content;
using Android.OS;
using Time_Moment_Clock_GMTPC.Models;

namespace Time_Moment_Clock_GMTPC.Platforms.Android
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (context == null || intent == null)
            {
                return;
            }

            AlarmPlatformScheduler.EnsureAlarmChannel(context);

            var alarmIdRaw = intent.GetStringExtra(AlarmPlatformScheduler.AlarmIdExtra);
            var label = intent.GetStringExtra(AlarmPlatformScheduler.AlarmLabelExtra) ?? "Bao thuc";
            var timeTicks = intent.GetLongExtra(AlarmPlatformScheduler.AlarmTimeTicksExtra, 0);

            var launchIntent = context.PackageManager?.GetLaunchIntentForPackage(context.PackageName!);
            var contentPendingIntent = launchIntent == null
                ? null
                : PendingIntent.GetActivity(context, 0, launchIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            Notification notification;
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                notification = new Notification.Builder(context, AlarmPlatformScheduler.AlarmChannelId)
                    .SetContentTitle("Alarm")
                    .SetContentText($"{label} dang reo")
                    .SetSmallIcon(Resource.Mipmap.appicon)
                    .SetAutoCancel(true)
                    .SetContentIntent(contentPendingIntent)
                    .Build()!;
            }
            else
            {
                notification = new Notification.Builder(context)
                    .SetContentTitle("Alarm")
                    .SetContentText($"{label} dang reo")
                    .SetSmallIcon(Resource.Mipmap.appicon)
                    .SetAutoCancel(true)
                    .SetContentIntent(contentPendingIntent)
                    .Build()!;
            }

            notification.Defaults = NotificationDefaults.Sound | NotificationDefaults.Vibrate;
            notification.Flags |= NotificationFlags.Insistent;

            var manager = (NotificationManager?)context.GetSystemService(Context.NotificationService);
            manager?.Notify(AlarmPlatformScheduler.GetRequestCode(Guid.TryParse(alarmIdRaw, out var alarmId) ? alarmId : Guid.NewGuid()), notification);

            if (timeTicks <= 0 || !Guid.TryParse(alarmIdRaw, out alarmId))
            {
                return;
            }

            var alarm = new AlarmItem
            {
                Id = alarmId,
                Label = label,
                Time = TimeSpan.FromTicks(timeTicks),
                IsEnabled = true
            };

            AlarmPlatformScheduler.ScheduleAlarm(context, alarm);
        }
    }
}
