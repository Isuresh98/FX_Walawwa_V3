using UnityEngine;

public class KeyItem : MonoBehaviour
{
    [SerializeField] private string keyID; // Unique Key ID (e.g., "Key_1")

   
    public void CollectKey()
    {
        PlayerPrefs.SetInt(keyID, 1); // Save key as collected
        Debug.Log("Collected key: " + keyID);
       // Destroy(gameObject); // Remove the key from the scene
    }
}