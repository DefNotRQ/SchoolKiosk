namespace SchoolKiosk;

internal readonly record struct KioskReadinessState(
    bool Overlay,
    bool ExactAlarms,
    bool Notifications,
    bool BatteryExemption,
    bool LockTask,
    bool BackgroundLaunchConfirmed,
    bool DeviceOwner)
{
    public bool IsReady => Overlay && ExactAlarms && Notifications && BatteryExemption && LockTask && BackgroundLaunchConfirmed;
}
