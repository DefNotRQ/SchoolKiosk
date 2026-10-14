using Android.Content;
using System.Globalization;
using System.Security.Cryptography;

namespace SchoolKiosk;

internal sealed class KioskConfig
{
    private readonly ISharedPreferences prefs;
    public KioskConfig(Context context)
    {
        prefs = context.GetSharedPreferences("kiosk", FileCreationMode.Private)!;
        EnsureAccessCodes();
    }

    public string Url { get => prefs.GetString("url", "")!; set => Put("url", value); }
    public int StartMinutes => SchoolSchedule.StartMinutes;
    public int EndMinutes => SchoolSchedule.EndMinutes;
    public int ActiveDays => 0x3E;
    public string Pin { get => prefs.GetString("pin", "")!; set => Put("pin", value); }
    public string SkipTodayCode { get => prefs.GetString("skipTodayCode", "")!; set => Put("skipTodayCode", value); }
    public string TemporaryUnlockCode { get => prefs.GetString("temporaryUnlockCode", "")!; set => Put("temporaryUnlockCode", value); }
    public string SkippedDate { get => prefs.GetString("skippedDate", "")!; private set => Put("skippedDate", value); }
    public long TemporaryUnlockUntilUnixMs
    {
        get => prefs.GetLong("temporaryUnlockUntil", 0);
        private set => Put("temporaryUnlockUntil", Math.Max(0, value));
    }
    public bool SetupComplete { get => prefs.GetBoolean("setup", false); set => Put("setup", value); }
    public bool BackgroundLaunchConfirmed { get => prefs.GetBoolean("backgroundLaunchConfirmed", false); set => Put("backgroundLaunchConfirmed", value); }
    public int Brightness { get => prefs.GetInt("brightness", 180); set => Put("brightness", Math.Clamp(value, 10, 255)); }
    public bool AdaptiveBrightness { get => prefs.GetBoolean("adaptiveBrightness", true); set => Put("adaptiveBrightness", value); }
    public int FailedPinAttempts
    {
        get => prefs.GetInt("failedPinAttempts", 0);
        set => prefs.Edit()!.PutInt("failedPinAttempts", Math.Clamp(value, 0, 4))!.Commit();
    }
    public long PinLockedUntilUnixMs
    {
        get => prefs.GetLong("pinLockedUntil", 0);
        set => prefs.Edit()!.PutLong("pinLockedUntil", Math.Max(0, value))!.Commit();
    }

    public bool IsKioskTime(DateTime now)
    {
        return KioskAccessPolicy.ShouldLock(now, IsScheduledKioskTime(now), SkippedDate, TemporaryUnlockUntilUnixMs);
    }

    public bool IsScheduledKioskTime(DateTime now) => SchoolSchedule.IsActive(now);
    public bool IsTodaySkipped(DateTime now) => KioskAccessPolicy.IsTodaySkipped(now, SkippedDate);
    public bool IsTemporaryUnlockActive(DateTime now) => KioskAccessPolicy.IsTemporaryUnlockActive(now, TemporaryUnlockUntilUnixMs);
    public void SkipToday(DateTime now)
    {
        SkippedDate = KioskAccessPolicy.LocalDateKey(now);
        TemporaryUnlockUntilUnixMs = 0;
    }
    public void StartTemporaryUnlock(DateTime now)
    {
        SkippedDate = "";
        TemporaryUnlockUntilUnixMs = new DateTimeOffset(now.Add(KioskAccessPolicy.TemporaryUnlockDuration)).ToUnixTimeMilliseconds();
    }
    public void CancelTemporaryUnlock() => TemporaryUnlockUntilUnixMs = 0;
    public void ClearAccessOverrides()
    {
        SkippedDate = "";
        TemporaryUnlockUntilUnixMs = 0;
    }

    public bool IsDayEnabled(DayOfWeek day) => (ActiveDays & (1 << (int)day)) != 0;

    public string ScheduleText => $"{SelectedDaysText()} {Format(StartMinutes)}–{Format(EndMinutes)}";

    public string SelectedDaysText()
    {
        if (ActiveDays == 0x7F) return "Every day";
        if (ActiveDays == 0) return "No days";
        var names = new[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        return string.Join(", ", Enumerable.Range(0, 7).Where(i => (ActiveDays & (1 << i)) != 0).Select(i => names[i]));
    }

    public string NextChangeText(DateTime now)
    {
        var target = SchoolSchedule.NextChange(now);
        var remaining = target - now;
        if (remaining.TotalHours >= 1)
            return $"{(int)remaining.TotalHours}h {remaining.Minutes}m";
        return $"{Math.Max(1, (int)Math.Ceiling(remaining.TotalMinutes))}m";
    }
    public static string Format(int minutes) => $"{minutes / 60:00}:{minutes % 60:00}";
    private void Put(string key, string value) => prefs.Edit()!.PutString(key, value)!.Apply();
    private void Put(string key, int value) => prefs.Edit()!.PutInt(key, value)!.Apply();
    private void Put(string key, long value) => prefs.Edit()!.PutLong(key, value)!.Apply();
    private void Put(string key, bool value) => prefs.Edit()!.PutBoolean(key, value)!.Apply();

    private void EnsureAccessCodes()
    {
        var skip = prefs.GetString("skipTodayCode", "")!;
        var temporary = prefs.GetString("temporaryUnlockCode", "")!;
        if (skip.Length >= 4 && temporary.Length >= 4 && skip != temporary) return;

        if (skip.Length < 4) skip = GenerateCode();
        do { temporary = GenerateCode(); } while (temporary == skip);
        prefs.Edit()!
            .PutString("skipTodayCode", skip)!
            .PutString("temporaryUnlockCode", temporary)!
            .Commit();
    }

    private static string GenerateCode() => RandomNumberGenerator.GetInt32(100000, 1000000).ToString(CultureInfo.InvariantCulture);
}
