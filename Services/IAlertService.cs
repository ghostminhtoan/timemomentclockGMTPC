namespace Time_Moment_Clock_GMTPC.Services
{
    public interface IAlertService
    {
        Task NotifyAsync(string title, string message);
    }
}
