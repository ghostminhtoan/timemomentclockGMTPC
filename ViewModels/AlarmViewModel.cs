using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.Json;
using Time_Moment_Clock_GMTPC.Models;
using Time_Moment_Clock_GMTPC.Services;

namespace Time_Moment_Clock_GMTPC.ViewModels
{
    public partial class AlarmViewModel : FloatingViewModelBase
    {
        private const string AlarmStorageKey = "saved_alarms";
        private readonly IAlertService _alertService;
        private readonly IAlarmSchedulerService _alarmSchedulerService;
        private readonly IDispatcherTimer _timer;

        [ObservableProperty]
        private TimeSpan draftAlarmTime = new(DateTime.Now.Hour, (DateTime.Now.Minute + 1) % 60, 0);

        [ObservableProperty]
        private string draftLabel = "Bao thuc moi";

        public ObservableCollection<AlarmItem> Alarms { get; } = new();

        public AlarmViewModel(
            IWindowManagerService windowManagerService,
            IAlertService alertService,
            IAlarmSchedulerService alarmSchedulerService)
            : base(windowManagerService)
        {
            Title = "Alarm";
            _alertService = alertService;
            _alarmSchedulerService = alarmSchedulerService;
            LoadAlarms();
            Alarms.CollectionChanged += OnAlarmsCollectionChanged;
            _timer = Application.Current!.Dispatcher.CreateTimer();
            if (!_alarmSchedulerService.SupportsBackgroundScheduling)
            {
                _timer.Interval = TimeSpan.FromSeconds(1);
                _timer.Tick += async (_, _) => await CheckAlarmsAsync();
                _timer.Start();
            }
        }

        public bool SupportsBackgroundScheduling => _alarmSchedulerService.SupportsBackgroundScheduling;

        [RelayCommand]
        private void AddAlarm()
        {
            var alarm = new AlarmItem
            {
                Time = DraftAlarmTime,
                Label = string.IsNullOrWhiteSpace(DraftLabel) ? "Bao thuc moi" : DraftLabel.Trim(),
                IsEnabled = true
            };

            AttachAlarm(alarm);
            Alarms.Add(alarm);
            SaveAlarms();
            _ = SyncSchedulerAsync();
        }

        [RelayCommand]
        private void DeleteAlarm(AlarmItem? alarm)
        {
            if (alarm == null)
            {
                return;
            }

            DetachAlarm(alarm);
            Alarms.Remove(alarm);
            SaveAlarms();
            _ = SyncSchedulerAsync();
        }

        [RelayCommand]
        private async Task PrepareAlarmPermissionsAsync()
        {
            await _alarmSchedulerService.EnsurePermissionsAsync();
            await SyncSchedulerAsync();
        }

        private async Task CheckAlarmsAsync()
        {
            var now = DateTime.Now;
            foreach (var alarm in Alarms.Where(x => x.IsEnabled))
            {
                var triggerWindowStart = now.Date.Add(alarm.Time);
                var triggerWindowEnd = triggerWindowStart.AddMinutes(1);

                if (now < triggerWindowStart || now >= triggerWindowEnd)
                {
                    continue;
                }

                if (alarm.LastTriggeredDate == now.Date)
                {
                    continue;
                }

                alarm.LastTriggeredDate = now.Date;
                await _alertService.NotifyAsync("Alarm", $"{alarm.Label} luc {alarm.DisplayTime}");
            }
        }

        private void LoadAlarms()
        {
            var raw = Preferences.Default.Get(AlarmStorageKey, string.Empty);
            if (string.IsNullOrWhiteSpace(raw))
            {
                return;
            }

            var items = JsonSerializer.Deserialize<List<AlarmItem>>(raw);
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                AttachAlarm(item);
                Alarms.Add(item);
            }
        }

        private void SaveAlarms()
        {
            Preferences.Default.Set(AlarmStorageKey, JsonSerializer.Serialize(Alarms));
        }

        private void OnAlarmsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (AlarmItem alarm in e.OldItems)
                {
                    DetachAlarm(alarm);
                }
            }

            if (e.NewItems != null)
            {
                foreach (AlarmItem alarm in e.NewItems)
                {
                    AttachAlarm(alarm);
                }
            }

            SaveAlarms();
            _ = SyncSchedulerAsync();
        }

        private void AttachAlarm(AlarmItem alarm)
        {
            alarm.PropertyChanged -= OnAlarmChanged;
            alarm.PropertyChanged += OnAlarmChanged;
        }

        private void DetachAlarm(AlarmItem alarm)
        {
            alarm.PropertyChanged -= OnAlarmChanged;
        }

        private void OnAlarmChanged(object? sender, PropertyChangedEventArgs e)
        {
            SaveAlarms();
            _ = SyncSchedulerAsync();
        }

        private Task SyncSchedulerAsync()
        {
            return _alarmSchedulerService.SyncAsync(Alarms);
        }
    }
}
