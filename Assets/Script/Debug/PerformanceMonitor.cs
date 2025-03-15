using UnityEngine;
using UnityEngine.UI;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class PerformanceMonitor : MonoBehaviour
{
    public Text infoText; // Assign a UI Text to display all performance stats

    private float deltaTime = 0.0f;
    private long totalRAM;
    private float cpuUsage;
    private float batteryLevel;
    private float batteryTemperature;
    private long availableStorage;
    private bool isLowEndDevice;

    void Start()
    {
        totalRAM = GetTotalRAM();
        isLowEndDevice = totalRAM <= 2048; // Check if device has ≤ 2GB RAM
    }

    void Update()
    {
        // FPS Calculation
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        // RAM Usage Calculation
        long unityMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024); // MB

        // Get CPU Usage
        cpuUsage = GetCPUUsage();

        // Get Battery Level & Temperature
        batteryLevel = GetBatteryLevel();
        batteryTemperature = GetBatteryTemperature();

        // Get Available Storage
        availableStorage = GetAvailableStorage();

        // Detect Lag (FPS < 30)
        string lagWarning = fps < 30 ? "⚠ Lag detected!" : "";

        // Display Information
        if (infoText != null)
        {
            infoText.text =
                $" **Performance Monitor**\n" +
                $" FPS: {Mathf.Round(fps)} {lagWarning}\n" +
                $" CPU Usage: {cpuUsage}%\n" +
                $" RAM Usage: {unityMemory} MB / {totalRAM} MB\n" +
                $" Battery: {batteryLevel}% | Temp: {batteryTemperature}°C\n" +
                $" Free Storage: {availableStorage} MB\n" +
                $" Device Type: {(isLowEndDevice ? "Low-End (⚠)" : "OK ")}";
        }
    }

    // ** Get Total RAM on Android (Uses Android API) **
    long GetTotalRAM()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                using (AndroidJavaObject memoryInfo = new AndroidJavaObject("android.app.ActivityManager$MemoryInfo"))
                using (AndroidJavaObject activityManager = activity.Call<AndroidJavaObject>("getSystemService", "activity"))
                {
                    activityManager.Call("getMemoryInfo", memoryInfo);
                    return memoryInfo.Get<long>("totalMem") / (1024 * 1024); // Convert bytes to MB
                }
            }
            catch (Exception e)
            {
                Debug.LogError("RAM detection failed: " + e.Message);
            }
        }
        return SystemInfo.systemMemorySize;
    }

    // ** Get CPU Usage (Experimental) **
    float GetCPUUsage()
    {
        if (SystemInfo.processorCount > 0)
        {
            return Mathf.Clamp(SystemInfo.processorFrequency / 1000f, 0, 100);
        }
        return 0;
    }

    // ** Get Battery Level on Mobile Devices **
    float GetBatteryLevel()
    {
        return SystemInfo.batteryLevel * 100f; // Returns 0-1, so multiply by 100
    }

    // ** Get Battery Temperature on Android **
    float GetBatteryTemperature()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                using (AndroidJavaClass batteryClass = new AndroidJavaClass("android.os.BatteryManager"))
                using (AndroidJavaObject context = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
                using (AndroidJavaObject intentFilter = new AndroidJavaObject("android.content.IntentFilter", "android.intent.action.BATTERY_CHANGED"))
                using (AndroidJavaObject intent = context.Call<AndroidJavaObject>("registerReceiver", null, intentFilter))
                {
                    return intent.Call<int>("getIntExtra", "temperature", -1) / 10.0f;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Battery temperature detection failed: " + e.Message);
            }
        }
        return -1;
    }

    // ** Get Available Storage on Android **
    long GetAvailableStorage()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                using (AndroidJavaClass environment = new AndroidJavaClass("android.os.Environment"))
                using (AndroidJavaObject statFs = new AndroidJavaObject("android.os.StatFs", environment.CallStatic<string>("getExternalStorageDirectory")))
                {
                    long availableBlocks = statFs.Call<long>("getAvailableBlocksLong");
                    long blockSize = statFs.Call<long>("getBlockSizeLong");
                    return (availableBlocks * blockSize) / (1024 * 1024); // Convert bytes to MB
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Storage detection failed: " + e.Message);
            }
        }
        return -1;
    }
}