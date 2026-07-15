using Microsoft.Extensions.DependencyInjection;
using Time_Moment_Clock_GMTPC.ViewModels;

namespace Time_Moment_Clock_GMTPC.Pages
{
    public partial class StopwatchPage : ContentPage
    {
        public StopwatchPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindingContext ??= MauiProgram.Services.GetRequiredService<StopwatchViewModel>();
        }
    }
}
