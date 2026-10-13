# Device setup

## Normal installation

A normal APK installation uses Android screen pinning. It is useful for evaluation, but Android still owns the system pinning dialog and unpin gesture.

1. Enable installation from the source you use to transfer the APK.
2. Install and open School Kiosk.
3. Enter the HTTPS website and a private administrator PIN.
4. Grant exact-alarm and display-over-other-apps access when requested.
5. On Honor/MagicOS, open **Settings → Battery → App launch**, disable automatic management for School Kiosk, and enable auto-launch, secondary launch, and run in background.
6. Test start and end boundaries before deployment.

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
