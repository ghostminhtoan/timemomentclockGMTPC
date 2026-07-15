using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Storage;
using System.Text;

namespace Time_Moment_Clock_GMTPC
{
    public partial class App : Application
    {
        private Exception? _startupException;

        public App()
        {
            try
            {
                InitializeComponent();
                Resources ??= new ResourceDictionary();
                Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("Resources/Styles/Colors.xaml", UriKind.Relative) });
                Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("Resources/Styles/Styles.xaml", UriKind.Relative) });
            }
            catch (Exception ex)
            {
                _startupException = ex;
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            try
            {
                if (_startupException != null)
                {
                    throw _startupException;
                }

                return new Window(MauiProgram.Services.GetRequiredService<AppShell>());
            }
            catch (Exception ex)
            {
                var page = new ContentPage
                {
                    Title = "Startup Error",
                    Content = new ScrollView
                    {
                        Content = new VerticalStackLayout
                        {
                            Padding = 24,
                            Spacing = 12,
                            Children =
                            {
                                new Label { Text = "App khoi dong that bai", FontSize = 24, FontAttributes = FontAttributes.Bold },
                                new Label { Text = ex.ToString(), FontSize = 12 }
                            }
                        }
                    }
                };

                try
                {
                    var path = Path.Combine(FileSystem.Current.AppDataDirectory, "startup-error.txt");
                    File.WriteAllText(path, new StringBuilder().AppendLine(DateTime.Now.ToString("O")).AppendLine(ex.ToString()).ToString());
                }
                catch
                {
                }

                return new Window(page);
            }
        }
    }
}
