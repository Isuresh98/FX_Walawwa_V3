using UnityEngine;
using UnityEngine.UI;
using System;
using Debug = UnityEngine.Debug;

public class PerformanceMonitor : MonoBehaviour
{
    public Text infoText;
    public Text lagReasonInfoText;

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
        isLowEndDevice = totalRAM <= 2048; // Devices with ≤2GB RAM are considered low-end

    
    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        long unityMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024); // MB
        cpuUsage = GetCPUUsage();
        batteryLevel = GetBatteryLevel();
        batteryTemperature = GetBatteryTemperature();
        availableStorage = GetAvailableStorage();

        // Lag Detection with Detailed Reasons
        string lagReason = "No significant lag detected ✅";
        string solution = "Game is running fine.";

        if (fps < 30)
        {
            if (cpuUsage > 80)
            {
                lagReason = "⚠ High CPU Usage";
                solution = "Close background apps & optimize game logic. Reduce AI updates, disable unnecessary scripts.";
            }
            else if (unityMemory > totalRAM * 0.8f)
            {
                lagReason = "⚠ High RAM Usage";
                solution = "Reduce texture resolution, optimize UI elements, limit particle effects.";
            }
            else if (availableStorage < 500)
            {
                lagReason = "⚠ Low Storage";
                solution = "Free up device storage. Cached files might be causing slowdowns.";
            }
            else if (batteryTemperature > 45)
            {
                lagReason = "⚠ Overheating Device";
                solution = "Reduce screen brightness, close background apps, and let the device cool down.";
            }
            else if (isLowEndDevice)
            {
                lagReason = "⚠ Low-End Device Performance";
                solution = "Enable low-quality mode. Reduce draw calls, disable shadows, use sprite atlases.";
            }
            else if (QualitySettings.vSyncCount > 0)
            {
                lagReason = "⚠ VSync Enabled";
                solution = "Disable VSync in quality settings to improve FPS.";
            }
            else if (Application.targetFrameRate < 30)
            {
                lagReason = "⚠ Low Target Frame Rate";
                solution = "Increase Application.targetFrameRate to at least 60 for smoother performance.";
            }
            else
            {
                lagReason = "⚠ Unknown Performance Issue";
                solution = "Try lowering graphics settings & enabling GPU instancing.";
            }
        }

        // Display Information in UI
        if (infoText != null)
        {
            infoText.text =
                $" **Performance Monitor**\n" +
                $" FPS: {Mathf.Round(fps)} {(fps < 30 ? "⚠ Lag detected!" : "")}\n" +
                $" CPU Usage: {cpuUsage}%\n" +
                $" RAM Usage: {unityMemory} MB / {totalRAM} MB\n" +
                $" Battery: {batteryLevel}% | Temp: {batteryTemperature}°C\n" +
                $" Free Storage: {availableStorage} MB\n" +
                $" Device Type: {(isLowEndDevice ? "Low-End (⚠)" : "OK ")}";
        }

        if (lagReasonInfoText != null)
        {
            lagReasonInfoText.text =
                $" **Lag Analysis**\n" +
                $" Reason: {lagReason}\n" +
                $" Solution: {solution}";
        }
#if UNITY_EDITOR
        // Debug Log Output
        Debug.Log($"[PerformanceMonitor] FPS: {Mathf.Round(fps)} | CPU: {cpuUsage}% | RAM: {unityMemory}MB/{totalRAM}MB | Battery: {batteryLevel}% | Temp: {batteryTemperature}°C | Storage: {availableStorage}MB");
#endif

        if (fps < 30)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[PerformanceMonitor] Lag Detected! Reason: {lagReason} | Solution: {solution}");
#endif
        }
    }

    // ** Get Total RAM on Android **
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
                    return memoryInfo.Get<long>("totalMem") / (1024 * 1024);
                }
            }
            catch (Exception e)
            {
#if UNITY_EDITOR

                Debug.LogError("RAM detection failed: " + e.Message);

#endif
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
        return SystemInfo.batteryLevel * 100f;
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
#if UNITY_EDITOR

                Debug.LogError("Battery temperature detection failed: " + e.Message);

#endif
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
                using (AndroidJavaObject statFs = new AndroidJavaObject("android.os.StatFs", environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").Call<string>("getAbsolutePath")))
                {
                    long availableBlocks = statFs.Call<long>("getAvailableBlocksLong");
                    long blockSize = statFs.Call<long>("getBlockSizeLong");
                    long availableSpace = (availableBlocks * blockSize) / (1024 * 1024);
#if UNITY_EDITOR
                    Debug.Log($"Available Storage: {availableSpace} MB");
#endif

                    return availableSpace;
                }
            }
            catch (Exception e)
            {
#if UNITY_EDITOR

                Debug.LogError("Storage detection failed: " + e.Message);
#endif

            }
        }
        return -1;
    }
}