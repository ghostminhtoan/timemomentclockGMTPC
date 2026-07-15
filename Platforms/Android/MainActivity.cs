using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Time_Moment_Clock_GMTPC
{
    [Activity(
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop,
        SupportsPictureInPicture = true,
        ResizeableActivity = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnUserLeaveHint()
        {
            base.OnUserLeaveHint();

            if (OperatingSystem.IsAndroidVersionAtLeast(26) && !IsInPictureInPictureMode)
            {
                EnterPictureInPictureMode(new PictureInPictureParams.Builder().Build()!);
            }
        }
    }
}
