namespace SchoolKiosk;

internal static class KioskAccessPolicy
{
    public static readonly TimeSpan TemporaryUnlockDuration = TimeSpan.FromMinutes(15);

    public static string LocalDateKey(DateTime now) => now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

    public static bool IsTodaySkipped(DateTime now, string? skippedDate) =>
        string.Equals(LocalDateKey(now), skippedDate, StringComparison.Ordinal);

    public static bool IsTemporaryUnlockActive(DateTime now, long unlockUntilUnixMs) =>
        unlockUntilUnixMs > new DateTimeOffset(now).ToUnixTimeMilliseconds();

    public static bool ShouldLock(DateTime now, bool scheduled, string? skippedDate, long unlockUntilUnixMs) =>
        scheduled && !IsTodaySkipped(now, skippedDate) && !IsTemporaryUnlockActive(now, unlockUntilUnixMs);
}
