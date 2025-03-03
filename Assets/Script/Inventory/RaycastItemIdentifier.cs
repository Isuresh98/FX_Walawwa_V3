using UnityEngine;
using TMPro;

public class RaycastItemIdentifier : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    private float displayDuration = 1f;
    private float timer;

    [SerializeField]
    private float rayDistance = 3f;

    public LayerMask detectableLayer;

    // Store raycast hits to avoid memory allocation
    private RaycastHit[] hits = new RaycastHit[1];

    // Update rate for mobile optimization
    private float checkInterval = 0.2f;
    private float checkTimer;

    void Update()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer < checkInterval) return;
        checkTimer = 0f;

        // Create a ray going straight forward from the camera
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        // Draw the ray in the Scene view for debugging
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);

        // Perform the raycast using RaycastNonAlloc for better performance on mobile
        int hitCount = Physics.RaycastNonAlloc(ray, hits, rayDistance, detectableLayer);

        if (hitCount > 0)
        {
            RaycastHit hit = hits[0];
            Item item = hit.collider.GetComponent<Item>();
            if (item != null)
            {
                // Display the item name on the UI if it has changed
                if (itemNameText.text != item.itemName)
                {
                    itemNameText.text = "Pickup " + item.itemName;
                }
                itemNameText.gameObject.SetActive(true);
                timer = displayDuration;
            }
        }

        // Hide the text after the timer runs out
        if (itemNameText.gameObject.activeSelf)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                itemNameText.gameObject.SetActive(false);
            }
        }
    }
}