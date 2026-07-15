using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using System.Threading.Tasks;
using Time_Moment_Clock_GMTPC.Services;

namespace Time_Moment_Clock_GMTPC.Platforms.Android
{
    public class WindowManagerService : IWindowManagerService
    {
        public string FloatingModeLabel => "Overlay / PiP";

        public bool KeepsToggleState
        {
            get
            {
                var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
                return activity != null && OperatingSystem.IsAndroidVersionAtLeast(23) && Settings.CanDrawOverlays(activity);
            }
        }

        public bool SupportsOverlayPermission => true;

        public Task SetFloatingModeAsync(bool enabled)
        {
            var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            if (activity == null)
            {
                return Task.CompletedTask;
            }

            if (KeepsToggleState)
            {
                var intent = new Intent(activity, typeof(OverlayClockService));
                intent.SetAction(enabled ? OverlayClockService.StartAction : OverlayClockService.StopAction);

                if (enabled)
                {
                    activity.StartForegroundService(intent);
                }
                else
                {
                    activity.StopService(intent);
                }

                return Task.CompletedTask;
            }

            if (!enabled || !OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                return Task.CompletedTask;
            }

            var builder = new PictureInPictureParams.Builder();
            activity.EnterPictureInPictureMode(builder.Build()!);
            return Task.CompletedTask;
        }

        public Task<bool> RequestOverlayPermissionAsync()
        {
            var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            if (activity == null)
            {
                return Task.FromResult(false);
            }

            if (!OperatingSystem.IsAndroidVersionAtLeast(23))
            {
                return Task.FromResult(false);
            }

            if (Settings.CanDrawOverlays(activity))
            {
                return Task.FromResult(true);
            }

            var intent = new Intent(Settings.ActionManageOverlayPermission);
            intent.SetData(global::Android.Net.Uri.Parse($"package:{activity.PackageName}"));
            intent.AddFlags(ActivityFlags.NewTask);
            activity.StartActivity(intent);
            return Task.FromResult(false);
        }
    }
}
