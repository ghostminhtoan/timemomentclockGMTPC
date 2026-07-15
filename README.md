# Time Moment Clock GMTPC

Multi-function clock app built with .NET MAUI, XAML, and C#.

## Features

- Real-time clock with 12h/24h format
- Stopwatch with lap support
- Countdown with speech/vibration alert
- Alarm list with saved alarms
- Windows always-on-top pin mode
- Android floating overlay and PiP fallback
- Android background alarms via `AlarmManager`

## Tech stack

- .NET MAUI
- CommunityToolkit.Maui
- CommunityToolkit.Mvvm
- XAML + C#

## Run

```powershell
dotnet build -f net10.0-windows10.0.19041.0
dotnet build -f net10.0-android
```

## Android notes

- Grant `Draw over other apps` to use overlay window.
- Grant notification permission on Android 13+.
- Grant exact alarm access on Android 12+ if the system asks for it.
- Alarm background scheduling is implemented with `AlarmManager` and a broadcast receiver.

## Windows notes

- `Pin / Float` uses WinUI `AppWindow` + `OverlappedPresenter.IsAlwaysOnTop`.
