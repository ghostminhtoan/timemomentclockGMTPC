using Microsoft.UI;
using Microsoft.UI.Windowing;
using Time_Moment_Clock_GMTPC.Services;
using WinRT.Interop;
using System;
using System.Threading.Tasks;

namespace Time_Moment_Clock_GMTPC.Platforms.Windows
{
    public class WindowManagerService : IWindowManagerService
    {
        public string FloatingModeLabel => "Always On Top";

        public bool KeepsToggleState => true;

        public bool SupportsOverlayPermission => false;

        public Task SetFloatingModeAsync(bool enabled)
        {
            var mauiWindow = Microsoft.Maui.Controls.Application.Current?.Windows[0].Handler?.PlatformView as MauiWinUIWindow;
            if (mauiWindow == null)
            {
                return Task.CompletedTask;
            }

            IntPtr windowHandle = WindowNative.GetWindowHandle(mauiWindow);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

            if (appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.IsAlwaysOnTop = enabled;
            }

            return Task.CompletedTask;
        }

        public Task<bool> RequestOverlayPermissionAsync()
        {
            return Task.FromResult(true);
        }
    }
}
