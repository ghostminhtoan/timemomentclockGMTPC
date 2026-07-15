using Android.App;
using Android.Content;
using Android.Media;
using Time_Moment_Clock_GMTPC.Models;

namespace Time_Moment_Clock_GMTPC.Platforms.Android
{
    internal static class AlarmPlatformScheduler
    {
        internal const string AlarmChannelId = "clock_alarm_channel";
        internal const string AlarmChannelName = "Clock Alarms";
        internal const string AlarmIdExtra = "alarm_id";
        internal const string AlarmLabelExtra = "alarm_label";
        internal const string AlarmTimeTicksExtra = "alarm_time_ticks";

        internal static void EnsureAlarmChannel(Context context)
        {
            if (!OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                return;
            }

            var manager = (NotificationManager?)context.GetSystemService(Context.NotificationService);
            if (manager?.GetNotificationChannel(AlarmChannelId) != null)
            {
                return;
            }

            var channel = new NotificationChannel(AlarmChannelId, AlarmChannelName, NotificationImportance.High)
            {
                Description = "Background alarm notifications"
            };
            channel.EnableVibration(true);
            channel.SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Alarm), null);
            manager?.CreateNotificationChannel(channel);
        }

        internal static void ScheduleAlarm(Context context, AlarmItem alarm)
        {
            if (!alarm.IsEnabled)
            {
                CancelAlarm(context, alarm.Id);
                return;
            }

            var triggerTime = GetNextTriggerTime(alarm.Time);
            var alarmManager = (AlarmManager?)context.GetSystemService(Context.AlarmService);
            var pendingIntent = BuildPendingIntent(context, alarm);
            var triggerUnixMs = new DateTimeOffset(triggerTime).ToUnixTimeMilliseconds();

            if (OperatingSystem.IsAndroidVersionAtLeast(23))
            {
                alarmManager?.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerUnixMs, pendingIntent);
            }
            else if (OperatingSystem.IsAndroidVersionAtLeast(19))
            {
                alarmManager?.SetExact(AlarmType.RtcWakeup, triggerUnixMs, pendingIntent);
            }
            else
            {
                alarmManager?.Set(AlarmType.RtcWakeup, triggerUnixMs, pendingIntent);
            }
        }

        internal static void CancelAlarm(Context context, Guid alarmId)
        {
            var alarmManager = (AlarmManager?)context.GetSystemService(Context.AlarmService);
            var pendingIntent = BuildPendingIntent(context, alarmId);
            alarmManager?.Cancel(pendingIntent);
            pendingIntent.Cancel();
        }

        internal static PendingIntent BuildPendingIntent(Context context, AlarmItem alarm)
        {
            var intent = new Intent(context, typeof(AlarmReceiver));
            intent.PutExtra(AlarmIdExtra, alarm.Id.ToString());
            intent.PutExtra(AlarmLabelExtra, alarm.Label);
            intent.PutExtra(AlarmTimeTicksExtra, alarm.Time.Ticks);
            return PendingIntent.GetBroadcast(
                context,
                GetRequestCode(alarm.Id),
                intent,
                PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable)!;
        }

        internal static PendingIntent BuildPendingIntent(Context context, Guid alarmId)
        {
            var intent = new Intent(context, typeof(AlarmReceiver));
            intent.PutExtra(AlarmIdExtra, alarmId.ToString());
            return PendingIntent.GetBroadcast(
                context,
                GetRequestCode(alarmId),
                intent,
                PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable)!;
        }

        internal static DateTime GetNextTriggerTime(TimeSpan alarmTime)
        {
            var now = DateTime.Now;
            var next = now.Date.Add(alarmTime);
            if (next <= now)
            {
                next = next.AddDays(1);
            }

            return next;
        }

        internal static int GetRequestCode(Guid alarmId)
        {
            return Math.Abs(alarmId.GetHashCode());
        }
    }
}
