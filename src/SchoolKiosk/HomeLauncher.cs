using Android.App;
using Android.Content;
using Android.Content.PM;

namespace SchoolKiosk;

internal static class HomeLauncher
{
    public static void Open(Activity activity)
    {
        try
        {
            var home = new Intent(Intent.ActionMain);
            home.AddCategory(Intent.CategoryHome);
            var candidates = activity.PackageManager?.QueryIntentActivities(home, PackageInfoFlags.MatchAll);
            var target = candidates?.FirstOrDefault(item =>
                item.ActivityInfo is not null && item.ActivityInfo.PackageName != activity.PackageName);
            if (target?.ActivityInfo is not null)
            {
                home.SetComponent(new ComponentName(target.ActivityInfo.PackageName!, target.ActivityInfo.Name!));
                home.AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop);
                activity.StartActivity(home);
                return;
            }
        }
        catch { }

        activity.MoveTaskToBack(true);
    }
}
