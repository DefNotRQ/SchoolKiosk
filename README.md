<p align="center">
  <img src="src/SchoolKiosk/Resources/drawable-nodpi/app_icon.png" width="112" alt="School Kiosk app icon">
</p>

<h1 align="center">School Kiosk</h1>

<p align="center">
  A focused, scheduled Android web kiosk built for dedicated school tablets.
</p>

<p align="center">
  <img alt="Android 8+" src="https://img.shields.io/badge/Android-8.0%2B-3DDC84?logo=android&logoColor=white">
  <img alt=".NET 10" src="https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white">
  <img alt="License: MIT" src="https://img.shields.io/badge/license-MIT-blue.svg">
</p>

School Kiosk turns an Android tablet into a scheduled, full-screen web browser. It is optimized for an Honor Pad 10 and is designed to recover its browser, schedule, and kiosk state after navigation attempts, process termination, or reboot.

Fresh installations contain **no default website and no default administrator PIN**. The administrator chooses both during first-run setup. Separate skip-today and 15-minute access codes are generated uniquely on each installation and can be changed from the protected settings panel.

## Download

Download the latest signed ARM64 APK from [GitHub Releases](https://github.com/DefNotRQ/SchoolKiosk/releases/latest). This works directly from a tablet browser; a PC is not required.

## Highlights

- Scheduled kiosk window: Monday–Friday, 08:30–15:30 in device-local time.
- Up to 20 WebView tabs, with inactive tabs paused to reduce memory use.
- One- and two-finger browsing with accidental three-or-more-finger gestures cancelled.
- Automatic WebView renderer recovery, connection status, loading progress, and offline retry screen.
- Wi-Fi and Bluetooth shortcuts plus manual and automatic brightness controls.
- Failed-PIN lockout and a five-minute administrator-session timeout.
- A skip-today code that suspends the current local date's kiosk session and automatically returns to the normal schedule the next day.
- A renewable 15-minute unlock code; reopening School Kiosk during the break immediately resumes scheduled locking.
- Foreground schedule monitor, exact boundary alarms, and boot recovery.
- Mandatory deployment-readiness gate: locking stays disabled until required permissions and device settings pass.
- Device-owner support for persistent Home routing, lock-task allowlisting, and stronger system UI restrictions.
- User-level screen-pinning fallback when device-owner provisioning is unavailable.

## Project layout

```text
SchoolKiosk/
├── src/SchoolKiosk/           Android application
├── tests/SchoolKiosk.Tests/   Schedule and boundary test executable
├── docs/                      Setup and architecture notes
├── CONTRIBUTING.md
├── SECURITY.md
└── LICENSE
```

## Requirements

- .NET 10 SDK
- .NET Android workload
- Android SDK API 36
- A 64-bit ARM Android tablet for the current release target

Install the Android workload:

```powershell
dotnet workload install android
```

## Build and test

Run the schedule tests:

```powershell
dotnet run --project tests/SchoolKiosk.Tests/SchoolKiosk.Tests.csproj -c Release
```

Build the APK:

```powershell
dotnet build src/SchoolKiosk/SchoolKiosk.csproj -c Release
```

Production APKs must be signed with your own private key. Never commit a keystore or its password. The repository intentionally contains no signing material or prebuilt APKs.

## Install

For a normal user-level installation:

```powershell
adb install path/to/com.ahura.schoolkiosk-Signed.apk
adb shell am start -n com.ahura.schoolkiosk/.MainActivity
```

Complete first-run setup by entering a full HTTPS URL and a private administrator PIN of at least four digits. Review or replace the two generated temporary-access codes before deployment; all three codes must be different.

For reliable dedicated-device restrictions, provision the app as device owner. See [Device setup](docs/DEVICE_SETUP.md).

## Android limitations

An ordinary APK cannot silently disable every system escape route. Screen pinning is controlled by Android and may display system guidance or allow a hardware unpin gesture. Strong kiosk controls require device-owner provisioning on a freshly reset tablet.

Some manufacturers also restrict background launches. The app blocks kiosk activation until its verifiable requirements pass and the administrator confirms the manufacturer-specific launch settings. See [Honor and Xiaomi setup](docs/DEVICE_SETUP.md#manufacturer-specific-background-settings).

## Privacy

The app has no analytics, advertisements, telemetry SDK, or remote management backend. Browsing traffic is handled by Android WebView and the configured website. Review the permissions and source before deploying it in a school environment.

## License

School Kiosk is available under the [MIT License](LICENSE).
