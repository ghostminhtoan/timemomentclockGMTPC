using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;

namespace Time_Moment_Clock_GMTPC.Platforms.Android
{
    [Service(Enabled = true, Exported = false)]
    public class OverlayClockService : Service
    {
        internal const string StartAction = "overlay_start";
        internal const string StopAction = "overlay_stop";
        private const string OverlayChannelId = "overlay_clock_channel";
        private const int OverlayNotificationId = 4041;

        private IWindowManager? _windowManager;
        private LinearLayout? _overlayView;
        private TextView? _timeText;
        private Handler? _handler;
        private Action? _tickAction;
        private WindowManagerLayoutParams? _layoutParams;
        private float _startX;
        private float _startY;
        private int _initialX;
        private int _initialY;

        public static bool IsRunning { get; private set; }

        public override IBinder? OnBind(Intent? intent) => null;

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            if (intent?.Action == StopAction)
            {
                StopSelf();
                return StartCommandResult.NotSticky;
            }

            if (!OperatingSystem.IsAndroidVersionAtLeast(23) || !Settings.CanDrawOverlays(this))
            {
                StopSelf();
                return StartCommandResult.NotSticky;
            }

            StartForeground(OverlayNotificationId, BuildForegroundNotification());
            CreateOverlayIfNeeded();
            IsRunning = true;
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            IsRunning = false;
            if (_overlayView != null && _windowManager != null)
            {
                _windowManager.RemoveView(_overlayView);
                _overlayView.Dispose();
            }

            if (_handler != null && _tickAction != null)
            {
                _handler.RemoveCallbacks(_tickAction);
            }
        }

        private Notification BuildForegroundNotification()
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                var manager = (NotificationManager?)GetSystemService(NotificationService);
                if (manager?.GetNotificationChannel(OverlayChannelId) == null)
                {
                    var channel = new NotificationChannel(OverlayChannelId, "Overlay Clock", NotificationImportance.Low);
                    manager?.CreateNotificationChannel(channel);
                }

                return new Notification.Builder(this, OverlayChannelId)
                    .SetContentTitle("Clock overlay")
                    .SetContentText("Dong ho dang noi tren man hinh")
                    .SetSmallIcon(Resource.Mipmap.appicon)
                    .SetOngoing(true)
                    .Build()!;
            }

            return new Notification.Builder(this)
                .SetContentTitle("Clock overlay")
                .SetContentText("Dong ho dang noi tren man hinh")
                .SetSmallIcon(Resource.Mipmap.appicon)
                .SetOngoing(true)
                .Build()!;
        }

        private void CreateOverlayIfNeeded()
        {
            if (_overlayView != null)
            {
                return;
            }

            _windowManager = GetSystemService(WindowService) as IWindowManager;
            if (_windowManager == null)
            {
                return;
            }

            var background = new GradientDrawable();
            background.SetColor(global::Android.Graphics.Color.Argb(225, 15, 23, 42));
            background.SetCornerRadius(42f);
            background.SetStroke(2, global::Android.Graphics.Color.Argb(255, 22, 119, 255));

            _overlayView = new LinearLayout(this)
            {
                Orientation = Orientation.Horizontal
            };
            _overlayView.SetPadding(32, 20, 20, 20);
            _overlayView.Background = background;

            _timeText = new TextView(this);
            _timeText.SetTextColor(global::Android.Graphics.Color.White);
            _timeText.TextSize = 20f;
            _timeText.Text = DateTime.Now.ToString("HH:mm:ss");
            _overlayView.AddView(_timeText);

            var closeButton = new TextView(this);
            closeButton.Text = "  X";
            closeButton.SetTextColor(global::Android.Graphics.Color.Argb(255, 142, 229, 210));
            closeButton.TextSize = 18f;
            closeButton.Click += (_, _) => StopSelf();
            _overlayView.AddView(closeButton);

            _layoutParams = new WindowManagerLayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent,
                OperatingSystem.IsAndroidVersionAtLeast(26) ? WindowManagerTypes.ApplicationOverlay : WindowManagerTypes.Phone,
                WindowManagerFlags.NotFocusable | WindowManagerFlags.LayoutInScreen,
                Format.Translucent)
            {
                Gravity = GravityFlags.Top | GravityFlags.Start,
                X = 40,
                Y = 160
            };

            _overlayView.Touch += OnOverlayTouch;
            _overlayView.Click += (_, _) => BringAppToFront();
            _windowManager.AddView(_overlayView, _layoutParams);

            _handler = new Handler(Looper.MainLooper!);
            _tickAction = () =>
            {
                if (_timeText != null)
                {
                    _timeText.Text = DateTime.Now.ToString("HH:mm:ss");
                }

                _handler?.PostDelayed(_tickAction!, 1000);
            };
            _handler.Post(_tickAction);
        }

        private void OnOverlayTouch(object? sender, global::Android.Views.View.TouchEventArgs e)
        {
            if (_layoutParams == null || _windowManager == null)
            {
                return;
            }

            switch (e.Event?.Action)
            {
                case MotionEventActions.Down:
                    _initialX = _layoutParams.X;
                    _initialY = _layoutParams.Y;
                    _startX = e.Event.RawX;
                    _startY = e.Event.RawY;
                    e.Handled = false;
                    break;
                case MotionEventActions.Move:
                    _layoutParams.X = _initialX + (int)(e.Event.RawX - _startX);
                    _layoutParams.Y = _initialY + (int)(e.Event.RawY - _startY);
                    _windowManager.UpdateViewLayout(_overlayView, _layoutParams);
                    e.Handled = true;
                    break;
                default:
                    e.Handled = false;
                    break;
            }
        }

        private void BringAppToFront()
        {
            var launchIntent = PackageManager?.GetLaunchIntentForPackage(PackageName!);
            if (launchIntent == null)
            {
                return;
            }

            launchIntent.AddFlags(ActivityFlags.NewTask | ActivityFlags.SingleTop);
            StartActivity(launchIntent);
        }
    }
}
