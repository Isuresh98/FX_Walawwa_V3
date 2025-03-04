using UnityEngine;
using UnityEngine.AI;

public class G_Enemy : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    public float followDistance = 10f;
    public float attackDistance = 2f;
    private float attackTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent.enabled = true; // Enable the agent
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if the player is within follow range
        if (distanceToPlayer <= followDistance)
        {
            agent.SetDestination(player.position);
            animator.SetFloat("Speed", agent.velocity.magnitude);
            if (distanceToPlayer <= attackDistance)
            {
                AttackPlayer();
            }
            else
            {
                print("Attac Not");
                animator.SetBool("isAttacking", false);
                animator.SetBool("isWalking", true);
            }
        }
        else
        {
            print("Attac Not USe");
            animator.SetBool("isWalking", false);
        }
    }

    void AttackPlayer()
    {
        print("Attac Start");
        agent.ResetPath(); // Stop moving
        animator.SetBool("isWalking", false);

        if (attackTimer <= 0f)
        {
            animator.SetBool("isAttacking", true);
            attackTimer = attackCooldown; // Reset cooldown
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }
    }
}