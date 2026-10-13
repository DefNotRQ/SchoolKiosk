using Android.App;
using Android.App.Admin;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;

namespace SchoolKiosk;

internal static class KioskReadiness
{
    public static KioskReadinessState Evaluate(Context context, KioskConfig? config = null)
    {
        config ??= new KioskConfig(context);
        var dpm = (DevicePolicyManager?)context.GetSystemService(Context.DevicePolicyService);
        var deviceOwner = dpm?.IsDeviceOwnerApp(context.PackageName) == true;
        var alarm = (AlarmManager?)context.GetSystemService(Context.AlarmService);
        var power = (PowerManager?)context.GetSystemService(Context.PowerService);

        var notifications = Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu ||
            context.CheckSelfPermission(Android.Manifest.Permission.PostNotifications) == Permission.Granted;
        var exactAlarms = Build.VERSION.SdkInt < BuildVersionCodes.S || alarm?.CanScheduleExactAlarms() == true;
        var batteryExemption = power?.IsIgnoringBatteryOptimizations(context.PackageName) == true;

        var lockTask = deviceOwner;
        if (!lockTask)
        {
            try
            {
                lockTask = Settings.Secure.GetInt(context.ContentResolver, "lock_to_app_enabled", 0) == 1;
            }
            catch { lockTask = false; }
        }

        return new KioskReadinessState(
            Build.VERSION.SdkInt < BuildVersionCodes.M || Settings.CanDrawOverlays(context),
            exactAlarms,
            notifications,
            batteryExemption,
            lockTask,
            config.BackgroundLaunchConfirmed,
            deviceOwner);
    }

    public static bool IsReady(Context context, KioskConfig? config = null)
        => Evaluate(context, config).IsReady;
}
