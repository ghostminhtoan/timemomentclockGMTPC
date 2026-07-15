using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace Time_Moment_Clock_GMTPC.Models
{
    public partial class AlarmItem : ObservableObject
    {
        [ObservableProperty]
        private Guid id = Guid.NewGuid();

        [ObservableProperty]
        private TimeSpan time = DateTime.Now.TimeOfDay;

        [ObservableProperty]
        private bool isEnabled = true;

        [ObservableProperty]
        private string label = "Bao thuc";

        [JsonIgnore]
        public DateTime LastTriggeredDate { get; set; } = DateTime.MinValue;

        [JsonIgnore]
        public string DisplayTime => DateTime.Today.Add(Time).ToString("HH:mm");

        [JsonIgnore]
        public string StatusText => IsEnabled ? "Dang bat" : "Da tat";

        partial void OnTimeChanged(TimeSpan value)
        {
            OnPropertyChanged(nameof(DisplayTime));
        }

        partial void OnIsEnabledChanged(bool value)
        {
            OnPropertyChanged(nameof(StatusText));
        }
    }
}
