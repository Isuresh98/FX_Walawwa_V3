using System.Collections;
using UnityEngine;

public class FristTrigger : MonoBehaviour
{
    public GameObject[] gravityObject;
   
    public bool isFrist_Trigger = false;
    public float enemyFallowTime=5f;
    //LightTrigger_1
    public Animator[] LightSet;
    public GameObject DamageOnSpotLight;
    private AudioSource audioSource;
    public AudioClip foodstep;
    public AudioClip ScardAndDamage;

    void Start()
    {
        // Optional: Initialization logic
        DamageOnSpotLight.SetActive(value:false);
        audioSource = GetComponent<AudioSource>();
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


        audioSource.PlayOneShot(foodstep);
        yield return new WaitForSeconds(1.1f);
      
        // Toggle LightTrigger for each Animator in LightSet
        // Toggle LightTrigger for each Animator in LightSet
        foreach (Animator anim in LightSet)
        {
            if (anim != null)
            {
                anim.SetTrigger("LightTrigger");
#if UNITY_EDITOR

                Debug.Log("LightTrigger toggled on: " + anim.gameObject.name);

#endif
            }
            else
            {
#if UNITY_EDITOR

                Debug.LogWarning("Animator reference is missing in LightSet");

#endif
            }
        }
    
        yield return new WaitForSeconds(delay);

        audioSource.PlayOneShot(ScardAndDamage);
        DamageOnSpotLight.SetActive(value: true);
        // Loop through all gravityObjects and set isKinematic to false
        foreach (GameObject obj in gravityObject)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
            else
            {
#if UNITY_EDITOR

                Debug.LogWarning("No Rigidbody found on " + obj.name);

#endif
            }
        }

     
        yield return new WaitForSeconds(delay);

        isFrist_Trigger = true;
#if UNITY_EDITOR

        Debug.Log("isFrist_Trigger is now true");

#endif
    }
}