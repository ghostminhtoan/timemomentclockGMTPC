using Microsoft.Maui.ApplicationModel;

namespace Time_Moment_Clock_GMTPC.Services
{
    public class AlertService : IAlertService
    {
        public async Task NotifyAsync(string title, string message)
        {
            try
            {
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(350));
            }
            catch
            {
            }

            try
            {
                await TextToSpeech.Default.SpeakAsync(message);
            }
            catch
            {
            }

            if (Application.Current?.Windows.FirstOrDefault()?.Page is Page page)
            {
                await page.DisplayAlertAsync(title, message, "OK");
            }
        }
    }
}
