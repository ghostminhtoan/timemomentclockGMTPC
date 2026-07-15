using Microsoft.Extensions.DependencyInjection;
using Time_Moment_Clock_GMTPC.ViewModels;

namespace Time_Moment_Clock_GMTPC.Pages
{
    public partial class AlarmPage : ContentPage
    {
        public AlarmPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindingContext ??= MauiProgram.Services.GetRequiredService<AlarmViewModel>();
        }
    }
}
