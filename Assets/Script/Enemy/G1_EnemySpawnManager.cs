using UnityEngine;

public class G1_EnemySpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints; // Assign 3 spawn points in Inspector

    private GameObject currentEnemy;
    public Transform FristPoss;
    [SerializeField] S1_EnemySpawnManager S1_enemySpawnManager;
    private void Start()
    {
        fristStart();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HandleEnemySpawn();
        }
    }

  public  void fristStart()
    {
        // Check if enemy already exists
        if (currentEnemy != null)
        {
#if UNITY_EDITOR

            Debug.Log("Enemy already present. No new spawn.");
#endif

            return;
        }

      
        // Spawn new enemy
        currentEnemy = Instantiate(enemyPrefab, FristPoss.position, Quaternion.identity);
#if UNITY_EDITOR
        Debug.Log("Spawned new enemy at " + FristPoss.name);
#endif
    }


    void HandleEnemySpawn()
    {
        S1_enemySpawnManager.DestroyEnemy();
    //   FristTrigger_1 FristTrigger = GameObject.FindGameObjectWithTag("Frist_Trigger").GetComponent<FristTrigger_1>();

        // Check if enemy already exists
        if (currentEnemy != null)
        {
#if UNITY_EDITOR
            Debug.Log("Enemy already present. No new spawn.");
#endif
            return;
        }

        // Pick a random spawn point
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        // Spawn new enemy
        currentEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
#if UNITY_EDITOR
        Debug.Log("Spawned new enemy at " + spawnPoint.name);
#endif
    }

    void Update()
    {
        // Clean reference if enemy is destroyed
        if (currentEnemy == null)
        {
            currentEnemy = null;
        }
    }
    // ✅ NEW: Destroy the currently spawned enemy
    public void DestroyEnemy()
    {
        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
            currentEnemy = null;
#if UNITY_EDITOR
            Debug.Log("Enemy destroyed.");
#endif
        }
    }
}