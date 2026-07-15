using CommunityToolkit.Mvvm.ComponentModel;

namespace Time_Moment_Clock_GMTPC.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private bool isBusy;
    }
}
