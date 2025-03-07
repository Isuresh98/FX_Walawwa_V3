using UnityEngine;

public class MosterShoot : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float shootForce = 10f;
    private Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (player.position - shootPoint.position).normalized;
            rb.AddForce(direction * shootForce, ForceMode.Impulse);
        }
    }
}
