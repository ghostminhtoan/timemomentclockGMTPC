using Time_Moment_Clock_GMTPC.Models;

namespace Time_Moment_Clock_GMTPC.Services
{
    public class FallbackAlarmSchedulerService : IAlarmSchedulerService
    {
        public bool SupportsBackgroundScheduling => false;

        public Task EnsurePermissionsAsync()
        {
            return Task.CompletedTask;
        }

        public Task SyncAsync(IEnumerable<AlarmItem> alarms)
        {
            return Task.CompletedTask;
        }
    }
}
