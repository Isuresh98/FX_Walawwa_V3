using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float lifetime = 5f; // Auto-destroy time
    private bool hasHit = false; // Prevent multiple hits

    private void Start()
    {
        Destroy(gameObject, lifetime); // Destroy if not hit anything after 5s
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return; // If already hit something, do nothing
        hasHit = true; // Mark as hit to prevent multiple triggers

        if (other.CompareTag("Player")) // Check if it hits the player
        {
#if UNITY_EDITOR

            Debug.Log("Bullet hit player...");

#endif
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(110);
        }

        Destroy(gameObject); // Destroy bullet after first valid hit
    }
}