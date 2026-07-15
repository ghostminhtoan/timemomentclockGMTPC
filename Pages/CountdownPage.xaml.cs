using Microsoft.Extensions.DependencyInjection;
using Time_Moment_Clock_GMTPC.ViewModels;

namespace Time_Moment_Clock_GMTPC.Pages
{
    public partial class CountdownPage : ContentPage
    {
        public CountdownPage()
        {
            InitializeComponent();
            BindingContext = MauiProgram.Services.GetRequiredService<CountdownViewModel>();
        }
    }
}
