namespace Time_Moment_Clock_GMTPC.Services
{
    public interface IWindowManagerService
    {
        string FloatingModeLabel { get; }

        bool KeepsToggleState { get; }

        bool SupportsOverlayPermission { get; }

        Task SetFloatingModeAsync(bool enabled);

        Task<bool> RequestOverlayPermissionAsync();
    }
}
