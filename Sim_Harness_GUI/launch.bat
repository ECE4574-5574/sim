"C:\Users\bt2016\AppData\Local\Android\android-sdk\platform-tools\adb" shell pm uninstall com.homeAutomationApp
"C:\Users\bt2016\AppData\Local\Android\android-sdk\platform-tools\adb" push %1 /data/local/
"C:\Users\bt2016\AppData\Local\Android\android-sdk\platform-tools\adb" push %2 /data/local/
"C:\Users\bt2016\AppData\Local\Android\android-sdk\platform-tools\adb" shell pm install /data/local/com.homeAutomationApp.apk
"C:\Users\bt2016\AppData\Local\Android\android-sdk\platform-tools\adb" shell am start -n com.homeAutomationApp/homeautomationapp.droid.MainActivity
"C:\Users\bt2016\AppData\Local\Android\android-sdk\platform-tools\adb" shell am start -n com.homeAutomationApp/homeautomationapp.droid.MainActivity
