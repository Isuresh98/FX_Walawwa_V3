using UnityEngine;

public class Keyhole : MonoBehaviour
{
    public string requiredKey; // The key needed for this keyhole
    [SerializeField] private DoorSystem linkedDoor; // The door this keyhole is linked to
    public bool locket;
    private void Start()
    {
        requiredKey = linkedDoor.requiredKey;
        locket = linkedDoor.isLocked;
    }


    public string TryUnlockDoor()
    {
        if (linkedDoor == null)
        {
            return "⚠️ No door assigned to this keyhole!";
        }

        if (linkedDoor.isLocked) // Check if the door is locked
        {
            if (PlayerPrefs.GetInt(requiredKey, 0) == 1) // Player has the key
            {
                linkedDoor.TryUnlockDoor(); // Unlock the door
                locket = linkedDoor.isLocked;
                return "🔑 Key used! Door unlocked.";

            }
            else
            {
                return "🔒 You need **" + requiredKey + "** to unlock this door!";
            }
        }
        else
        {
            return "🚪 The door is already unlocked!";
        }
    }
}