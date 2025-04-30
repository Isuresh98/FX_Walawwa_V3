using NUnit.Framework.Interfaces;
using UnityEngine;

public class KickTrigger : MonoBehaviour
{
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
            if (rb != null)
            {
                rb.isKinematic = false; // Enable physics
                Vector3 kickDirection = transform.forward;
                rb.AddForce(kickDirection * kickForce);
                isUnlockend = true;
            }
        }
        else
        {
#if UNITY_EDITOR

            Debug.Log($"Not Unlock Plank");

#endif
        }


    }
}