using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CerventAI_1 : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    public float detectionRange = 15f;  // Enemy detects player within this range
    public float runRange; // Adjust this value as needed
    public float attackRange = 1.2f; // Distance at which enemy starts attacking
    public float attackCooldown = 2f; // Time between attacks

    private bool isAttacking = false;
    private float lastAttackTime = 0f;
    private bool isWalking = false;

    [Header("Waypoint Patrol Settings")]
    public Transform[] waypoints;
    public float patrolWaitTime = 2f; // Time to wait at each waypoint
    private int currentWaypointIndex = 0;
    private bool isPatrolling = false;


    [Header("Frist Step")]
    ServentenmeyFristTrigger FristTrigger;

    [Header("Coin Detect Section")]
    public float CoindetectionRange = 1f;
    public float waitTime = 10f;
    public LayerMask coinLayer;
    private Transform detectedCoin;
    private bool isWaiting = false;
    private bool isRun = false;
    public bool IsCoinDetected = false;
    private bool isWaitingCoroutineStarted = false;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        FristTrigger = GameObject.FindGameObjectWithTag("Severnt_Frist_Trigger").GetComponent<ServentenmeyFristTrigger>();
        if (player == null)
        {
#if UNITY_EDITOR

            Debug.LogError("Player not found! Make sure your player GameObject has the 'Player' tag.");

#endif
            return;
        }
        runRange = detectionRange / 2;
        agent.enabled = false;
    }

    void Update()
    {
        if (player == null) return;
        if (!FristTrigger.isFrist_Trigger) return;

        DetectCoin();


        if (IsCoinDetected && !isWaiting)
        {
            MissingInteractionPlayer();
        }
        else if (!IsCoinDetected && !isWaiting)
        {
            IntracPlayer();
        }


    }


    private void IntracPlayer()
    {
        agent.enabled = true;
        // Check if the player's position is on the NavMesh
        if (!IsPlayerOnNavMesh(player.position))
        {
            // Debug.Log("Player is in a non-walkable area. Stopping pursuit.");
            SetPlayerOutOfRange();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        //        Debug.Log($"Distance to Player: {distanceToPlayer}");

        if (distanceToPlayer <= detectionRange)
        {
            LookAtPlayer(); // ---------------------------------------------------🔹 Rotate to face player
            agent.SetDestination(player.position);
            //   Debug.Log("Chasing player...");

            if (distanceToPlayer > attackRange)
            {
                agent.isStopped = false;
                isAttacking = false;
                animator.SetBool("isAttacking", false);
                if (distanceToPlayer <= runRange)
                {
                    isWalking = false;
                    isRun = true;
                    animator.SetBool("isRun", true);
                }
                else
                {
                    isWalking = true;
                    isRun = false;
                    animator.SetBool("isRun", false);
                }
            }
            else
            {
                agent.isStopped = true;



                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    isWalking = true;
                    isRun = false;
                    animator.SetBool("isRun", false);
                    lastAttackTime = Time.time;
                    isAttacking = true;
                    animator.SetBool("isAttacking", true);

                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;

                    // Stop movement by setting MoveSpeed to 0
                    animator.SetFloat("MoveSpeed", 0);
#if UNITY_EDITOR

                    Debug.Log("Enemy is attacking!");

#endif
                }


            }

            UpdateMovement();
        }

        else
        {
            SetPlayerOutOfRange();
        }
    }
    private void MissingInteractionPlayer()
    {
        if (detectedCoin != null)
        {
#if UNITY_EDITOR

            Debug.Log("set enemy coin coin place");

#endif
            agent.SetDestination(detectedCoin.position);
            if (Vector3.Distance(transform.position, detectedCoin.position) < 1.5f && !isWaitingCoroutineStarted)
            {
#if UNITY_EDITOR


                Debug.Log("set enemy waiting coin coin place");

#endif
                StartCoroutine(WaitAtCoin());
            }
        }
    }


    void DetectCoin()
    {
        if (isWaiting) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, CoindetectionRange, coinLayer);
        if (hitColliders.Length > 0)
        {
            detectedCoin = hitColliders[0].transform;
            IsCoinDetected = true;
        }
        else
        {
            detectedCoin = null;
            IsCoinDetected = false;
        }
    }

    IEnumerator WaitAtCoin()
    {
        isWaitingCoroutineStarted = true;
        isWaiting = true;
        agent.isStopped = true;

        // Change coin's layer to ignore it
        if (detectedCoin != null)
        {
            detectedCoin.gameObject.layer = LayerMask.NameToLayer("IgnoredCoin"); // Ensure this layer exists in Unity
        }
        //enemy behavior
        agent.isStopped = true;
        agent.velocity = Vector3.zero; // Stop completely
        animator.SetFloat("MoveSpeed", 0);
        // Play "stand down" animation
        animator.SetBool("standDown", true);
        yield return new WaitForSeconds(0.1f);
        // Wait until "stand down" animation finishes
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

        // Play "coin collect" animation
        animator.SetBool("standDown", false);
        animator.SetBool("collectCoin", true);

        yield return new WaitForSeconds(waitTime);

        animator.SetBool("collectCoin", false);
        animator.SetBool("standUp", true);
        // Wait until "coin collect" animation finishes
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        animator.SetBool("standUp", false);
        animator.SetBool("standUp_V2", true);
        // Wait until "stand up" animation finishes
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

        animator.SetBool("standUp_V2", false);
        animator.SetBool("standUp_V3", true);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

        isWaitingCoroutineStarted = false;
        detectedCoin = null;
        IsCoinDetected = false;
        agent.isStopped = false;
        isWaiting = false;
    }


    private void UpdateMovement()
    {
        if (isAttacking)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            animator.SetFloat("MoveSpeed", 0);
        }
        else if (isRun)
        {
            agent.isStopped = false;
            agent.speed = 2.5f; // Adjust speed for running
            animator.SetFloat("MoveSpeed", 1);
        }
        else if (isWalking)
        {
            agent.isStopped = false;
            agent.speed = 1.5f; // Adjust speed for walking
            animator.SetFloat("MoveSpeed", 0.5f);
        }
        else
        {
            float speed = agent.velocity.magnitude;
            speed = Mathf.Clamp(speed, 0, 1);
            animator.SetFloat("MoveSpeed", speed, 0.2f, Time.deltaTime);
        }
    }

    private bool IsPlayerOnNavMesh(Vector3 position)
    {
        NavMeshHit hit;
        bool isOnNavMesh = NavMesh.SamplePosition(position, out hit, 2.0f, NavMesh.AllAreas);

        // Debug.DrawRay(hit.position, Vector3.up * 2, Color.green, 2.0f); // Show NavMesh hit position
        //Debug.DrawRay(position, Vector3.up * 2, Color.red, 2.0f); // Show Player position

        if (!isOnNavMesh)
        {
            // Debug.LogError("Player is NOT on NavMesh! Stopping pursuit.");
            return false;
        }

        float distanceThreshold = 1.0f; // Allow some tolerance
        if (Vector3.Distance(position, hit.position) > distanceThreshold)
        {
            //  Debug.LogWarning($"Player is slightly off NavMesh by {Vector3.Distance(position, hit.position)} units. Adjusting...");
            return true; // Allow movement but recognize slight offset
        }

        return true;
    }

    private void SetPlayerOutOfRange()
    {
        Vector3 lastPlayerPosition = agent.destination; // Store last reached player position

        agent.isStopped = true;
        isAttacking = false;
        animator.SetBool("isAttacking", false);
        animator.SetFloat("MoveSpeed", 0);
#if UNITY_EDITOR

        Debug.Log("Player out of range. Moving to last known position before patrolling.");

#endif

        StartCoroutine(MoveToLastPlayerPosition(lastPlayerPosition));
    }
    /// 🔹 **NEW METHOD: Makes the enemy face the player**
    private void LookAtPlayer()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Prevents the enemy from tilting up/down
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }


    private IEnumerator MoveToLastPlayerPosition(Vector3 lastPosition)
    {
        agent.isStopped = false;
        agent.SetDestination(lastPosition);

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance + 0.1f)
        {
            animator.SetFloat("MoveSpeed", Mathf.Clamp(agent.velocity.magnitude, 0, 1));
            yield return null;
        }

        agent.isStopped = true;
        animator.SetFloat("MoveSpeed", 0);
        yield return new WaitForSeconds(1f); // Brief wait before starting patrol

        StartPatrol();
    }


    void StartPatrol()
    {
        if (waypoints.Length == 0) return;
        isPatrolling = true;
        StartCoroutine(PatrolRoutine());
    }


    IEnumerator PatrolRoutine()
    {
        while (isPatrolling)
        {
            if (waypoints.Length == 0) yield break;

            agent.SetDestination(waypoints[currentWaypointIndex].position);
            agent.isStopped = false;

            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance + 0.1f)
            {
                animator.SetFloat("MoveSpeed", Mathf.Clamp(agent.velocity.magnitude, 0, 1));
                yield return null;
            }

            agent.isStopped = true;
            animator.SetFloat("MoveSpeed", 0);
            yield return new WaitForSeconds(patrolWaitTime);

            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }
}
