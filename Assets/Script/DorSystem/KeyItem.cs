using UnityEngine;

public class KeyItem : MonoBehaviour
{
    public string keyID; // Unique Key ID (e.g., "Key_1")

    private void Start()
    {
        PlayerPrefs.SetInt(keyID, 0); // Save key as collected
    }

    public void CollectKey()
    {
        PlayerPrefs.SetInt(keyID, 1); // Save key as collected
#if UNITY_EDITOR
        Debug.Log("Collected key: " + keyID);

#endif
        // Destroy(gameObject); // Remove the key from the scene
    }

    public void DropKey()
    {
        PlayerPrefs.SetInt(keyID, 0); // Save key as collected
#if UNITY_EDITOR

        Debug.Log("Drop key: " + keyID);

#endif
    }
}