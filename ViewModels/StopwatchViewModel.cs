using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Dispatching;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Time_Moment_Clock_GMTPC.Services;

namespace Time_Moment_Clock_GMTPC.ViewModels
{
    public partial class StopwatchViewModel : FloatingViewModelBase
    {
        private readonly Stopwatch _stopwatch = new();
        private readonly IDispatcherTimer _timer;

        [ObservableProperty]
        private string elapsedTime = "00:00:00.00";

        public ObservableCollection<string> Laps { get; } = new();

        public StopwatchViewModel(IWindowManagerService windowManagerService)
            : base(windowManagerService)
        {
            Title = "Stopwatch";
            _timer = Application.Current!.Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(50);
            _timer.Tick += (_, _) => ElapsedTime = _stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.ff");
        }

        public bool IsRunning => _stopwatch.IsRunning;

        public string StartPauseText => IsRunning ? "Tam dung" : "Bat dau";

        [RelayCommand]
        private void StartPause()
        {
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                _timer.Stop();
            }
            else
            {
                _stopwatch.Start();
                _timer.Start();
            }

            OnPropertyChanged(nameof(IsRunning));
            OnPropertyChanged(nameof(StartPauseText));
        }

        [RelayCommand]
        private void Reset()
        {
            _stopwatch.Reset();
            _timer.Stop();
            Laps.Clear();
            ElapsedTime = "00:00:00.00";
            OnPropertyChanged(nameof(IsRunning));
            OnPropertyChanged(nameof(StartPauseText));
        }

        [RelayCommand]
        private void AddLap()
        {
            if (!_stopwatch.IsRunning)
            {
                return;
            }

            Laps.Insert(0, $"Lap {Laps.Count + 1}: {_stopwatch.Elapsed:hh\\:mm\\:ss\\.ff}");
        }
    }
}
