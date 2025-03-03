using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private PlayerHealth playerHealth;
    public int attackDamage = 10; // Damage dealt to player
    private Transform player;
    private AudioSource source;
    public AudioClip attacksound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>(); // Get the PlayerHealth component
        }
        source = GetComponent<AudioSource>();
    }
    private void PlayerDamage()
    {
        // Deal damage to player
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
        // Play attack sound only if it's not already playing
        if (!source.isPlaying)
        {
            source.PlayOneShot(attacksound);
        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
