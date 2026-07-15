using Time_Moment_Clock_GMTPC.Models;

namespace Time_Moment_Clock_GMTPC.Services
{
    public interface IAlarmSchedulerService
    {
        bool SupportsBackgroundScheduling { get; }

        Task EnsurePermissionsAsync();

        Task SyncAsync(IEnumerable<AlarmItem> alarms);
    }
}
