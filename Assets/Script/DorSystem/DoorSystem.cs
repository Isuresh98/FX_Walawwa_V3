using UnityEngine;

public class DoorSystem : MonoBehaviour
{
    [SerializeField] private string doorID; // Unique ID for each door
     public string requiredKey; // Key required to unlock
     public bool isLocked = true; // Door lock status
    [SerializeField] private Animator doorAnimator; // Animator for door open/close
    [SerializeField] private float closeDelay = 3f; // Time before door closes

    private bool isOpen = false; // Track door state

    private void Start()
    {
        PlayerPrefs.SetInt(doorID, 0); // Savedoor
        //// Load saved door state
        if (PlayerPrefs.GetInt(doorID, 0) == 1)
        {
          isLocked = false;
        }
    }

    // 🔹 Return Door Status as a String
    public string DoorStatus()
    {
        if (isLocked)
        {
            return "This door is LOCKED. You need **" + requiredKey + "** to unlock it.";
        }
        else
        {
            return "This door is UNLOCKED. You can enter.";
        }
    }

    public void ChaeckDoor()
    {
        
            if (isOpen)
            {
                CloseDoor();
            }
            else
            {
                TryUnlockDoor();
            }
        
    }

    private void OnTriggerEnter(Collider other)
    {
       
    }

    public void TryUnlockDoor()
    {
        if (!isLocked)
        {
           // OpenDoor();
            return;
        }

        if (PlayerPrefs.GetInt(requiredKey, 0) == 1) // Check if player has the key
        {
            isLocked = false;
            PlayerPrefs.SetInt(doorID, 1); // Save door state
           // OpenDoor();
        }
        else
        {
            Debug.Log("Door is locked! Find the key.");
        }
    }

    public void OpenDoor()
    {
        if (doorAnimator)
        {
            doorAnimator.SetTrigger("Open");
        }
        isOpen = true;
        Debug.Log("Door Opened!");
       // Invoke(nameof(CloseDoor), closeDelay); // Auto-close after delay
    }

    public void CloseDoor()
    {
        if (doorAnimator)
        {
            doorAnimator.SetTrigger("Close");
        }
        isOpen = false;
        Debug.Log("Door Closed!");
    }

    public void ToggleDoor()
    {
        print("Toggele Work");

        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }
}