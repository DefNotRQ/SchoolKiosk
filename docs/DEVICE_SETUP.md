# Device setup

## Normal installation

A normal APK installation uses Android screen pinning. It is useful for evaluation, but Android still owns the system pinning dialog and unpin gesture.

1. Enable installation from the source you use to transfer the APK.
2. Install and open School Kiosk.
3. Enter the HTTPS website and a private administrator PIN.
4. Complete every item in the deployment-readiness screen.
5. Test start and end boundaries before deployment.

## Deployment-readiness gate

School Kiosk deliberately refuses to start immersive mode, lock task, recovery takeovers, the foreground monitor, or schedule alarms until all required setup items pass:

- Display over other apps
- Exact alarms
- Notification permission
- Unrestricted battery use
- Android screen pinning, unless device-owner lock task is available
- Administrator confirmation of the manufacturer's background-launch settings

The readiness screen links to each relevant system page. Return to School Kiosk and select **Recheck everything** after changing a setting. Device-owner mode remains optional, but is strongly recommended because ordinary screen pinning cannot provide the same security guarantees.

## Manufacturer-specific background settings

Menu wording can differ slightly by model and operating-system release.

### Honor / MagicOS

1. Open **Settings** and search for **App launch** or **Launch**.
2. Select School Kiosk and turn off **Manage automatically**.
3. Enable **Auto-launch**, **Secondary launch**, and **Run in background**.
4. Search Settings for **Battery optimization**, show **All apps**, select School Kiosk, and choose **Don't allow** optimization.
5. Turn off Battery saver and Ultra power saving while relying on scheduled kiosk recovery.
6. Return to School Kiosk and confirm the manufacturer setup step.

These steps follow Honor's guidance for [allowing background launch](https://www.honor.com/sa-en/support/content/en-us00406923/) and [preventing background apps from being restarted](https://www.honor.com/sa-en/support/content/en-us00406916/).

### Xiaomi / Redmi / POCO — HyperOS or MIUI

1. Open **Settings → Apps → Permissions → Background autostart** and enable School Kiosk.
2. Open **Settings → Apps → School Kiosk → Power** and choose **No restrictions**.
3. Turn off Battery saver and Ultra battery saver while relying on scheduled kiosk recovery.
4. Open Recents, press and hold the School Kiosk card, and select the lock icon so cleanup does not remove it.
5. If available under the app's permissions, allow background pop-ups or opening new windows while running in the background.
6. Return to School Kiosk and confirm the manufacturer setup step.

Xiaomi documents the [Background autostart menu](https://www.mi.com/global/support/faq/details/KA-507608/) and recommends **No restrictions**, autostart, and locking the app in Recents for reliable background operation in its [background connection guidance](https://www.mi.com/global/support/faq/details/KA-700752/).

## Device-owner provisioning

Device-owner mode is Android's supported path for a dedicated kiosk. It requires a factory-reset device with no personal or work accounts already added.

> Device-owner provisioning can remove normal access to the tablet. Back up anything important first and test on a non-production device.

1. Factory-reset the tablet.
2. Complete initial setup without adding an account.
3. Enable Developer options and USB debugging.
4. Install your signed APK:

   ```powershell
   adb install SchoolKiosk.apk
   ```

5. Assign device ownership:

   ```powershell
   adb shell dpm set-device-owner com.ahura.schoolkiosk/.KioskAdminReceiver
   ```

6. Launch the application:

   ```powershell
   adb shell am start -n com.ahura.schoolkiosk/.MainActivity
   ```

7. Complete first-run setup and verify Home, Recents, system UI, reboot, offline recovery, and schedule transitions.

Removing device-owner mode may require ADB access or a factory reset. Confirm your recovery procedure before placing the tablet into service.
