using UnityEngine;

public class LightIntensityController : MonoBehaviour
{
    public Light targetLight;         // Reference to the Light component
    public float startIntensity = 5f; // Starting intensity (5)
    public float endIntensity = 1f;   // Ending intensity (1)
    public float duration = 0f;      // Time to decrease

    public float timer = 0f;
    public bool isDecreasing = false;

    void Start()
    {
        // Get the Light component on this GameObject
        targetLight = GetComponent<Light>();

        // Set the light's initial intensity to startIntensity (5)
        if (targetLight != null)
        {
            targetLight.intensity = endIntensity;
        }
    }

    void Update()
    {
        if (targetLight != null && isDecreasing)
        {
            // Increment the timer
            timer += Time.deltaTime;

            // Decrease intensity from startIntensity to endIntensity
            targetLight.intensity = Mathf.Lerp(startIntensity, endIntensity, timer / duration);

            // Check if it reached the endIntensity
            if (timer >= duration)
            {
                // Stop the cycle and reset
                isDecreasing = false;
               // timer = 0f;
                targetLight.intensity = endIntensity;
            }
        }
    }

    // Method to start the light intensity decrease
    public void StartDecrease()
    {
        if (!isDecreasing)
        {
            isDecreasing = true;
            timer = 0f; // Reset timer to start decreasing
        }
    }
    public void StopDecrease()
    {
        
            isDecreasing = false;
          
        
    }
}