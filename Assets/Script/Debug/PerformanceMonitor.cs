using UnityEngine;
using UnityEngine.UI;

public class PerformanceMonitor : MonoBehaviour
{
    public Text ramText;
    public Text fpsText;

    private float deltaTime = 0.0f;

    void Update()
    {
        // FPS Calculation
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        // RAM Usage Calculation
        long totalMemory = System.GC.GetTotalMemory(false) / (1024 * 1024); // In MB
        long unityMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024); // In MB

        // Update UI Text
        if (ramText != null)
            ramText.text = $"RAM: {totalMemory} MB (GC) | {unityMemory} MB (Unity)";

        if (fpsText != null)
            fpsText.text = $"FPS: {Mathf.Round(fps)}";
    }
}