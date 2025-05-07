using System.Collections;
using UnityEngine;

public class ServentenmeyFristTrigger : MonoBehaviour
{

    public bool isFrist_Trigger = false;
    public float enemyFallowTime = 5f;


    void Start()
    {

    }

    void Update()
    {
        // Optional: Update logic
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger has the tag "Player"
        if (other.CompareTag("Player"))
        {
            // Print "Set" to the console
#if UNITY_EDITOR
            Debug.Log("Set");
#endif
            if (!isFrist_Trigger)
            {
                // Start the coroutine to set isFrist_Trigger after 10 seconds
                StartCoroutine(SetTriggerAfterDelay(enemyFallowTime));
            }


        }
    }

    // Coroutine to wait for 10 seconds before setting isFrist_Trigger to true
    private IEnumerator SetTriggerAfterDelay(float delay)
    {


        yield return new WaitForSeconds(delay);

        isFrist_Trigger = true;
#if UNITY_EDITOR

        Debug.Log("isFrist_Trigger is now true");

#endif
    }
}
