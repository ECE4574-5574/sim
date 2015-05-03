#!/bin/bash

~/Library/Developer/Xamarin/android-sdk-macosx/platform-tools/adb shell pm uninstall com.homeAutomationApp
~/Library/Developer/Xamarin/android-sdk-macosx/platform-tools/adb push $1 /data/local/
~/Library/Developer/Xamarin/android-sdk-macosx/platform-tools/adb push $2 /data/local/
~/Library/Developer/Xamarin/android-sdk-macosx/platform-tools/adb shell pm install /data/local/com.homeAutomationApp.apk
~/Library/Developer/Xamarin/android-sdk-macosx/platform-tools/adb shell am start -n com.homeAutomationApp/homeautomationapp.droid.MainActivity
~/Library/Developer/Xamarin/android-sdk-macosx/platform-tools/adb shell am start -n com.homeAutomationApp/homeautomationapp.droid.MainActivity
