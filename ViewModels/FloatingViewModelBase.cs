using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Time_Moment_Clock_GMTPC.Services;

namespace Time_Moment_Clock_GMTPC.ViewModels
{
    public abstract partial class FloatingViewModelBase : BaseViewModel
    {
        protected readonly IWindowManagerService WindowManagerService;

        [ObservableProperty]
        private bool isPinned;

        protected FloatingViewModelBase(IWindowManagerService windowManagerService)
        {
            WindowManagerService = windowManagerService;
        }

        public string FloatingModeLabel => WindowManagerService.FloatingModeLabel;

        public bool SupportsOverlayPermission => WindowManagerService.SupportsOverlayPermission;

        public string FloatingActionText => WindowManagerService.KeepsToggleState && IsPinned ? "Bo ghim" : "Pin / Float";

        partial void OnIsPinnedChanged(bool value)
        {
            OnPropertyChanged(nameof(FloatingActionText));
        }

        [RelayCommand]
        protected async Task ToggleFloatingModeAsync()
        {
            var nextValue = WindowManagerService.KeepsToggleState ? !IsPinned : true;
            await WindowManagerService.SetFloatingModeAsync(nextValue);
            IsPinned = WindowManagerService.KeepsToggleState && nextValue;
        }

        [RelayCommand]
        protected Task RequestOverlayPermissionAsync()
        {
            return WindowManagerService.RequestOverlayPermissionAsync();
        }
    }
}
