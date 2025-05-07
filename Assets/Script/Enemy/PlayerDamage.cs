using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    PlayerHealth playerHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

  public void Takedamage()
    {
        playerHealth.TakeDamage(120);
    }
}
