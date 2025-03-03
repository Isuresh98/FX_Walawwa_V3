using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float followDistance = 10f;
    public float attackDistance = 2f;

    private NavMeshAgent agent;
    private float attackTimer;
    private Animator animator;

    [Header("Frist Step")]
    FristTrigger FristTrigger;
    public bool IsEnemy_1 = false;

    [Header("Player Attack Place")]
    public float attackCooldown = 2f;

    [Header("Waypoint Patrol Settings")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    public float waypointReachThreshold = 1f;
    private PlayerController playerController;
    private float waitTimer = 0f;
    public float waitTimeAtWaypoint = 2f;

    [Header("NavMesh Check Settings")]
    public float navMeshCheckRadius = 1f;
    public float offNavMeshDelay = 10f; // Delay before switching to waypoints
    private float playerOffNavMeshTimer = 0f;
    private bool isWaitingForPlayer = false;

    // Store last known position
    private Vector3 lastKnownPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;

        FristTrigger = GameObject.FindGameObjectWithTag("Frist_Trigger").GetComponent<FristTrigger>();
        animator = GetComponentInChildren<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
        if (animator)
        {
            animator.SetBool("isStanding", true);
            animator.SetBool("isWalking", false);
        }
    }

    void Update()
    {
        if (FristTrigger.isFrist_Trigger && !playerController.iSIntractableHideObject)
        {
            agent.enabled = true;
            agent.isStopped = false;

            if (!IsPlayerOnNavMesh())
            {
                // Store the last known position when the player is on the NavMesh
                lastKnownPosition = player.position;

                // Reset the timer and follow the player
                playerOffNavMeshTimer = 0f;
                isWaitingForPlayer = false;

                FallowEnemy_1();
            }
            else
            {
                // Start the delay timer if the player is off the NavMesh
                playerOffNavMeshTimer += Time.deltaTime;

                // Move to the last known position
                agent.SetDestination(lastKnownPosition);

                // Stop moving when reaching the last known position
                if (Vector3.Distance(transform.position, lastKnownPosition) <= waypointReachThreshold)
                {
                    agent.isStopped = true;
                    // Idle animation during waiting
                    if (animator)
                    {
                        animator.SetBool("isWalking", false);
                        animator.SetBool("isAttacking", false);

                    }
                }



                // Check if the timer exceeds the delay time
                if (playerOffNavMeshTimer >= offNavMeshDelay)
                {
                    isWaitingForPlayer = true;
                    PatrolWaypoints();  // Start patrolling after the delay
                }
            }
        }
        else if (playerController.iSIntractableHideObject)
        {
            PatrolWaypoints();
        }
    }

    public void StopFallow()
    {
        agent.isStopped = true;
        if (animator)
        {
            animator.SetBool("isStanding", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", false);
        }
    }

    void FallowEnemy_1()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackDistance)
        {
            // Stop and Attack
            agent.isStopped = true;
            AttackPlayer();
        }
        else if (distanceToPlayer <= followDistance)
        {
            // Follow Player
            agent.isStopped = false;
            agent.SetDestination(player.position);

            // Make the enemy face the player
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            if (animator)
            {
                animator.SetBool("isStanding", false);
                animator.SetBool("isWalking", true);
                animator.SetBool("isAttacking", false);
            }
        }
      
    }

    void AttackPlayer()
    {
        if (attackTimer <= 0f)
        {
            Debug.Log("Attacking Player");

            attackTimer = attackCooldown;

            if (animator)
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", true);
            }
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }
    }

    void PatrolWaypoints()
    {
        if (waypoints.Length == 0) return;

        // Check if the enemy reached the waypoint
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) <= waypointReachThreshold)
        {
            waitTimer += Time.deltaTime;
            agent.isStopped = true;

            if (waitTimer >= waitTimeAtWaypoint)
            {
                waitTimer = 0f;

                // Move to the next waypoint if not the last one
                if (currentWaypointIndex < waypoints.Length - 1)
                {
                    currentWaypointIndex++;
                }
                else
                {
                    // Stay at the last waypoint instead of looping back to 0
                    agent.isStopped = true;
                    return;
                }
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }

        if (animator)
        {
            animator.SetBool("isStanding", false);
            animator.SetBool("isWalking", !agent.isStopped);
            animator.SetBool("isAttacking", false);
        }
    }

    bool IsPlayerOnNavMesh()
    {
        NavMeshHit hit;

        // Check if the player's position is on a NavMesh surface
        if (!NavMesh.SamplePosition(player.position, out hit, navMeshCheckRadius, NavMesh.AllAreas))
        {
            Debug.Log("Player is in a non-walkable area!");
            return true; // Player is NOT on the NavMesh (Non-walkable area)
        }

        // Check if the area is walkable
        if (hit.mask == 0) // 0 means not assigned to any NavMesh area
        {
            Debug.Log("Player is in a non-walkable area (NavMesh hit but no valid mask)!");
            return true;
        }

        Debug.Log("Player is in a walkable area!");
        return false; // Player is on a walkable NavMesh
    }
}
