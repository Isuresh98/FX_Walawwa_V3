using UnityEngine;

public class InteractOutline : MonoBehaviour
{
    void Start()
    {
        // Find all GameObjects with the tag "Interact"
        GameObject[] interactObjects = GameObject.FindGameObjectsWithTag("Interact");

        // Loop through each object and add the Outline component
        foreach (GameObject obj in interactObjects)
        {
            // Check if the object already has an Outline component
            if (obj.GetComponent<Outline>() == null)
            {
                // Add Outline component
                var outline = obj.AddComponent<Outline>();

                // Set Outline properties
                outline.OutlineMode = Outline.Mode.OutlineVisible;
                outline.OutlineColor = Color.yellow;
                outline.OutlineWidth = 1f;
            }
        }
    }
}