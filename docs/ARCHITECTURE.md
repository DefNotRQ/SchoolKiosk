# Architecture

School Kiosk is a native .NET Android application built around one full-screen activity and Android WebView.

## Main components

- `MainActivity` owns the browser, custom setup/settings UI, tabs, connectivity feedback, brightness controls, PIN handling, and lock-task transitions.
- `ScheduleMonitorService` is a foreground service that watches the active schedule and recovers the kiosk when it loses the foreground.
- `ScheduleReceiver` creates exact alarms for the next schedule boundary.
- `BootReceiver` restores the monitor and alarm after reboot, package replacement, time changes, or timezone changes.
- `HomeRouterActivity` routes Home to the kiosk during its active window and to the system launcher outside it.
- `KioskAdminReceiver` exposes Android device-administration and device-owner functionality.
- `SchoolSchedule` is the dependency-free schedule model used by both the app and tests.

## Browser model

The application maintains at most 20 WebViews. Only the active WebView is visible and resumed; inactive tabs are hidden and paused. New-window requests create tabs until the limit is reached. Clearing tabs destroys every WebView and creates a fresh home tab.

## Kiosk levels

In user mode, the app uses Android screen pinning and repeated foreground recovery. This cannot provide the same guarantees as a managed device.

In device-owner mode, Android can allowlist the app for lock task, make it the persistent Home handler, and apply stronger restrictions to status-bar expansion and other system features.

## Schedule

The current schedule is compiled as Monday–Friday, 08:30–15:30 local time. `SchoolSchedule.NextChange` computes the next boundary, while the foreground monitor performs short-interval recovery checks during the active window.
