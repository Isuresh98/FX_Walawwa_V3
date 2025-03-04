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

        if (distanceToPlayer <= detectionRange)
        {
            agent.SetDestination(player.position);

            if (distanceToPlayer > stoppingDistance)
            {
                agent.isStopped = false;
                isAttacking = false;
                animator.SetBool("isAttacking", false);
            }
            else
            {
                agent.isStopped = true;

                // Attack logic
                if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
                {
                    lastAttackTime = Time.time;
                    isAttacking = true;
                    animator.SetBool("isAttacking", true);
                }
                else
                {
                    isAttacking = false;
                    animator.SetBool("isAttacking", false);
                }
            }
        }
        else
        {
            agent.isStopped = true;
            isAttacking = false;
            animator.SetBool("isAttacking", false);
        }

        // Set animation blend tree parameter based on movement speed
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }
}