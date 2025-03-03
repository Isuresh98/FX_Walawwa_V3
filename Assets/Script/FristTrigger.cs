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
            Debug.Log("Set");
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
                Debug.Log("LightTrigger toggled on: " + anim.gameObject.name);
            }
            else
            {
                Debug.LogWarning("Animator reference is missing in LightSet");
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
                Debug.LogWarning("No Rigidbody found on " + obj.name);
            }
        }

     
        yield return new WaitForSeconds(delay);

        isFrist_Trigger = true;
        Debug.Log("isFrist_Trigger is now true");
    }
}