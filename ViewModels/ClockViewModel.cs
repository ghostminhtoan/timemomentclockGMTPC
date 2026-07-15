using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Storage;
using Time_Moment_Clock_GMTPC.Services;

namespace Time_Moment_Clock_GMTPC.ViewModels
{
    public partial class ClockViewModel : FloatingViewModelBase
    {
        private const string Use24HourKey = "clock_use_24h";
        private readonly IDispatcherTimer _timer;

        [ObservableProperty]
        private string currentTime = "--:--:--";

        [ObservableProperty]
        private string currentDate = string.Empty;

        [ObservableProperty]
        private string timezoneLabel = string.Empty;

        [ObservableProperty]
        private bool is24HourFormat;

        public ClockViewModel(IWindowManagerService windowManagerService)
            : base(windowManagerService)
        {
            Title = "Clock";
            Is24HourFormat = Preferences.Default.Get(Use24HourKey, true);
            timezoneLabel = TimeZoneInfo.Local.DisplayName;
            _timer = Application.Current!.Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (_, _) => UpdateTime();
            _timer.Start();
            UpdateTime();
        }

        partial void OnIs24HourFormatChanged(bool value)
        {
            Preferences.Default.Set(Use24HourKey, value);
            UpdateTime();
        }

        private void UpdateTime()
        {
            var now = DateTime.Now;
            CurrentTime = now.ToString(Is24HourFormat ? "HH:mm:ss" : "hh:mm:ss tt");
            CurrentDate = now.ToString("dddd, dd MMMM yyyy");
        }
    }
}
