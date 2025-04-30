using NUnit.Framework.Interfaces;
using UnityEngine;

public class KickTrigger : MonoBehaviour
{
    public Transform kickPoint; // Assign this in the inspector to be the upper center of the door
    public float kickForce = 500f;
    private Rigidbody rb;
    public int PlankCount;
    public int PlankMaxCount;
    public bool isUnlock = false;
    public bool isUnlockend = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        isUnlock = false;
        isUnlockend = false;
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            kikdoor();
        }
    }
    public void addPlank(int Count)
    {
        PlankCount += Count;

        if (PlankCount >= PlankMaxCount)
        {
            isUnlock = true;
        }
    }
    public void kikdoor()
    {
        if (PlankCount >= PlankMaxCount)
        {
            if (rb != null && kickPoint != null)
            {
                rb.isKinematic = false;

                Vector3 kickDirection = -transform.up;
                rb.AddForceAtPosition(kickDirection * kickForce, kickPoint.position);

                isUnlockend = true;
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("Not Unlock Plank");
#endif
        }
    }


}
