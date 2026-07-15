using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Dispatching;
using Time_Moment_Clock_GMTPC.Services;

namespace Time_Moment_Clock_GMTPC.ViewModels
{
    public partial class CountdownViewModel : FloatingViewModelBase
    {
        private readonly IAlertService _alertService;
        private readonly IDispatcherTimer _timer;
        private TimeSpan _initialDuration;
        private TimeSpan _remainingDuration;

        [ObservableProperty]
        private string hoursText = "00";

        [ObservableProperty]
        private string minutesText = "05";

        [ObservableProperty]
        private string secondsText = "00";

        [ObservableProperty]
        private string remainingText = "00:05:00";

        [ObservableProperty]
        private double progress;

        [ObservableProperty]
        private bool isRunning;

        public CountdownViewModel(IWindowManagerService windowManagerService, IAlertService alertService)
            : base(windowManagerService)
        {
            Title = "Countdown";
            _alertService = alertService;
            _timer = Application.Current!.Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += async (_, _) => await TickAsync();
        }

        public string StartPauseText => IsRunning ? "Tam dung" : "Bat dau";

        partial void OnIsRunningChanged(bool value)
        {
            OnPropertyChanged(nameof(StartPauseText));
        }

        [RelayCommand]
        private void QuickPreset(string value)
        {
            if (value == "1m")
            {
                HoursText = "00";
                MinutesText = "01";
                SecondsText = "00";
            }
            else if (value == "5m")
            {
                HoursText = "00";
                MinutesText = "05";
                SecondsText = "00";
            }
            else if (value == "10m")
            {
                HoursText = "00";
                MinutesText = "10";
                SecondsText = "00";
            }
        }

        [RelayCommand]
        private void StartPause()
        {
            if (IsRunning)
            {
                _timer.Stop();
                IsRunning = false;
                return;
            }

            if (_remainingDuration == TimeSpan.Zero)
            {
                _initialDuration = BuildDuration();
                _remainingDuration = _initialDuration;
                RemainingText = _remainingDuration.ToString(@"hh\:mm\:ss");
                Progress = 0;
            }

            if (_remainingDuration == TimeSpan.Zero)
            {
                return;
            }

            _timer.Start();
            IsRunning = true;
        }

        [RelayCommand]
        private void Reset()
        {
            _timer.Stop();
            _initialDuration = TimeSpan.Zero;
            _remainingDuration = TimeSpan.Zero;
            RemainingText = "00:00:00";
            Progress = 0;
            IsRunning = false;
        }

        private async Task TickAsync()
        {
            if (_remainingDuration <= TimeSpan.Zero)
            {
                return;
            }

            _remainingDuration -= TimeSpan.FromSeconds(1);
            RemainingText = _remainingDuration.ToString(@"hh\:mm\:ss");
            Progress = _initialDuration == TimeSpan.Zero ? 0 : 1 - (_remainingDuration.TotalSeconds / _initialDuration.TotalSeconds);

            if (_remainingDuration > TimeSpan.Zero)
            {
                return;
            }

            _timer.Stop();
            IsRunning = false;
            await _alertService.NotifyAsync("Countdown", "Dem nguoc da ket thuc.");
            Reset();
        }

        private TimeSpan BuildDuration()
        {
            _ = int.TryParse(HoursText, out var hours);
            _ = int.TryParse(MinutesText, out var minutes);
            _ = int.TryParse(SecondsText, out var seconds);
            hours = Math.Max(0, hours);
            minutes = Math.Clamp(minutes, 0, 59);
            seconds = Math.Clamp(seconds, 0, 59);
            HoursText = hours.ToString("00");
            MinutesText = minutes.ToString("00");
            SecondsText = seconds.ToString("00");
            return new TimeSpan(hours, minutes, seconds);
        }
    }
}
