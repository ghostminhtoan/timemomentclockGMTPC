using Microsoft.Extensions.DependencyInjection;

namespace Time_Moment_Clock_GMTPC
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(MauiProgram.Services.GetRequiredService<AppShell>());
        }
    }
}
