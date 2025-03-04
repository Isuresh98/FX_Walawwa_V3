using UnityEngine;
using UnityEngine.AI;

public class G_Enemy : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    public float detectionRange = 15f;  // Enemy detects player within this range
    public float stoppingDistance = 1.5f; // Distance at which enemy stops moving
    public float attackRange = 1.2f; // Distance at which enemy starts attacking
    public float attackCooldown = 2f; // Time between attacks

    private bool isAttacking = false;
    private float lastAttackTime = 0f;


    [Header("Waypoint Patrol Settings")]
    public Transform[] waypoints;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure your player GameObject has the 'Player' tag.");
            return;
        }

        agent.enabled = true;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Debug.Log($"Distance to Player: {distanceToPlayer}");

        if (distanceToPlayer <= detectionRange)
        {
            agent.SetDestination(player.position);
            Debug.Log("Chasing player...");

            if (distanceToPlayer > stoppingDistance)
            {
                agent.isStopped = false;
                isAttacking = false;
                animator.SetBool("isAttacking", false);
                Debug.Log("Moving towards player...");
            }
            else
            {
                agent.isStopped = true;
                Debug.Log("Reached stopping distance...");

                if (distanceToPlayer <= attackRange)
                {
                    if (Time.time >= lastAttackTime + attackCooldown)
                    {
                        lastAttackTime = Time.time;
                        isAttacking = true;
                        animator.SetBool("isAttacking", true);

                        agent.isStopped = true;
                        agent.velocity = Vector3.zero;

                        // Stop movement by setting MoveSpeed to 0
                        animator.SetFloat("MoveSpeed", 0);
                        Debug.Log("Enemy is attacking!");
                    }
                }
                else
                {
                    isAttacking = false;
                    animator.SetBool("isAttacking", false);

                    // Restore movement speed based on NavMeshAgent velocity
                    float speed = agent.velocity.magnitude;
                    animator.SetFloat("MoveSpeed", speed);
                }
            }

            UpdateMovement();
        }
        else
        {
            Debug.Log("Player out of range. Stopping.");
            agent.isStopped = true;
            isAttacking = false;
            animator.SetBool("isAttacking", false);
        }
    }

    private void UpdateMovement()
    {
        if (isAttacking)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero; // Stop completely
            animator.SetFloat("MoveSpeed", 0);
            Debug.Log("Stopping movement for attack.");
        }
        else
        {
            float speed = agent.velocity.magnitude;
            speed = Mathf.Clamp(speed, 0, 1); // Ensure speed is within range

            animator.SetFloat("MoveSpeed", speed, 0.2f, Time.deltaTime);
            Debug.Log($"Setting MoveSpeed: {speed}");
        }
    }
}