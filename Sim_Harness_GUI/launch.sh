#!/bin/bash

~/Library/Developer/Xamarin/android-sdk-macosx/platform-tools/adb shell pm uninstall com.homeAutomationApp
~/Library/Developer/Xamarin/android-sdk-macosx/platform-tools/adb push com.homeAutomationApp.apk /data/local/
~/Library/Developer/Xamarin/android-sdk-macosx/platform-tools/adb push adb.sh /data/local/
~/Library/Developer/Xamarin/android-sdk-macosx/platform-tools/adb shell pm install /data/local/com.homeAutomationApp.apk
~/Library/Developer/Xamarin/android-sdk-macosx/platform-tools/adb shell am start -n com.homeAutomationApp/homeautomationapp.droid.MainActivity
~/Library/Developer/Xamarin/android-sdk-macosx/platform-tools/adb shell am start -n com.homeAutomationApp/homeautomationapp.droid.MainActivity
