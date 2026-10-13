using SchoolKiosk;

var checks = 0;
void Check(bool value, string name) { checks++; if (!value) throw new Exception(name); }
var monday = new DateTime(2026, 10, 12);
for (var day = 0; day < 14; day++)
for (var minute = 0; minute < 1440; minute++)
{
    var now = monday.AddDays(day).AddMinutes(minute);
    var expected = day % 7 < 5 && minute >= 510 && minute < 930;
    Check(SchoolSchedule.IsActive(now) == expected, $"active {now}");
    var next = SchoolSchedule.NextChange(now);
    Check(next > now, $"future {now}");
    Check(SchoolSchedule.IsActive(next.AddTicks(-1)) != SchoolSchedule.IsActive(next), $"boundary {next}");
}
Check(SchoolSchedule.NextChange(monday.AddDays(4).AddHours(16)) == monday.AddDays(7).AddHours(8).AddMinutes(30), "Friday skips weekend");
Check(!SchoolSchedule.IsActive(monday.AddHours(8).AddMinutes(30).AddTicks(-1)), "before start");
Check(!SchoolSchedule.IsActive(monday.AddHours(15).AddMinutes(30)), "end exclusive");

for (var mask = 0; mask < 64; mask++)
{
    var state = new KioskReadinessState(
        (mask & 1) != 0,
        (mask & 2) != 0,
        (mask & 4) != 0,
        (mask & 8) != 0,
        (mask & 16) != 0,
        (mask & 32) != 0,
        DeviceOwner: false);
    Check(state.IsReady == (mask == 63), $"readiness gate mask {mask}");
}
var ownerOnly = new KioskReadinessState(false, false, false, false, false, false, DeviceOwner: true);
Check(!ownerOnly.IsReady, "device owner does not bypass operational requirements");
Console.WriteLine($"PASS: {checks} schedule and readiness assertions.");
