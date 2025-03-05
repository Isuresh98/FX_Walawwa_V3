using UnityEngine;

public class CoinCollider : MonoBehaviour
{
    public bool isRayHit = false;
    private SphereCollider Collider;
    public float resetTimer = 0.1f; // Time to reset after no hit
    public float timer = 0f;

    void Start()
    {
        Collider = GetComponent<SphereCollider>();
    }

    void Update()
    {
        if (isRayHit)
        {
            Collider.radius = 0.5f;
            timer = resetTimer; // Reset the countdown timer
        }

        // Reduce timer if isRayHit is true, and reset it when time runs out
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                isRayHit = false; // Reset when timer runs out
                Collider.radius = 5f;
            }
        }
    }
}