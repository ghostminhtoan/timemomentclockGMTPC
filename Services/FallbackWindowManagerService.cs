namespace Time_Moment_Clock_GMTPC.Services
{
    public class FallbackWindowManagerService : IWindowManagerService
    {
        public string FloatingModeLabel => "Unsupported";

        public bool KeepsToggleState => false;

        public bool SupportsOverlayPermission => false;

        public Task SetFloatingModeAsync(bool enabled)
        {
            return Task.CompletedTask;
        }

        public Task<bool> RequestOverlayPermissionAsync()
        {
            return Task.FromResult(false);
        }
    }
}
