using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public Slider healthBar;
    private PlayerController player;
    private bool Isdie=false;
    private Camera mainCamera;
    public Transform deathCameraPoint; // Assign in the Inspector
    public float cameraMoveSpeed = 2f; // Speed of camera transition
    private bool moveCamera = false;


    void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
        player = FindObjectOfType<PlayerController>();
        mainCamera = Camera.main;
    }
    private void Update()
    {
        // Smoothly move the camera to the death point
        if (moveCamera && deathCameraPoint)
        {
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                deathCameraPoint.position,
                cameraMoveSpeed * Time.deltaTime
            );

            mainCamera.transform.rotation = Quaternion.Lerp(
                mainCamera.transform.rotation,
                deathCameraPoint.rotation,
                cameraMoveSpeed * Time.deltaTime
            );
        }
    }
    public void TakeDamage(int damage)
    {
       
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            Die();
            currentHealth = 0;
          
        }
        healthBar.value = currentHealth;

        if (currentHealth >= 50 && currentHealth <= 60)
        {
          
        }

       
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthBar.value = currentHealth;
    }

    void Die()
    {
#if UNITY_EDITOR

        Debug.Log("Player Died!");

#endif

        if (!Isdie)
        {
            // Start moving the camera to the death point
            moveCamera = true;

            // Delay the CatchPlayer method by 5 seconds
            Invoke("DelayedCatchPlayer", 0.5f);

            Isdie = true;
        }
    }

    void DelayedCatchPlayer()
    {
        player.CatchPlayer(2);
    }
}