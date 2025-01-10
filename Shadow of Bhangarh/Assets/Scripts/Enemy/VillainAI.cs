using System.Collections;
using UnityEngine;

public class VillainAI : MonoBehaviour
{
    [Header("Sound Detection")]
    UnityEngine.AI.NavMeshAgent navMeshAgent;
    public float moveSpeed = 3.5f;
    private Vector3 soundLocation;
    public bool isReturning = false;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isWaiting = false;
    private bool isDead = false;
    bool soundHeard = false;

    [Header("Enemy States")]
    public Transform player;
    public float detectionRadius = 7f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    float lastAttackTime = 0f;

    [Header("Navigation")]
    public Transform startPosition;

    private Animator animator;

    void Start()
    {
        Debug.Log("VillainAI initialized.");
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        animator = GetComponent<Animator>();

        // Store initial position if startPosition isn't set
        if (startPosition == null)
        {
            GameObject startMarker = new GameObject($"{gameObject.name}_StartPosition");
            startMarker.transform.position = transform.position;
            startPosition = startMarker.transform;
        }

        // Auto-assign player if not set
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
                Debug.Log("Player assigned automatically.");
            }
            else
            {
                Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
            }
        }
    }

    void Update()
    {
        if (isDead) return;

        Debug.Log($"Current state - Chasing: {isChasing}, Attacking: {isAttacking}, Returning: {isReturning}, Waiting: {isWaiting}");

        // Always check for player in range
        LookForPlayer();

        if (isAttacking)
        {
            AttackPlayer();
        }
        else if (isChasing)
        {
            ChasePlayer();
        }
        else if (isWaiting)
        {
            LookForPlayer(); // Continue looking for the player
        }
        else if (isReturning)
        {
            ReturnToStart();
        }

        UpdateAnimations();
    }

    void LookForPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Debug.Log($"Distance to player: {distanceToPlayer}");

        if (distanceToPlayer <= detectionRadius)
        {
            RaycastHit hit;
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRadius))
            {
                Debug.Log($"Raycast hit: {hit.transform.name}");
                if (hit.transform == player)
                {
                    Debug.Log("Player detected!");
                    isChasing = true;
                    isReturning = false;
                    isWaiting = false;
                    navMeshAgent.isStopped = false;
                }
            }
            else
            {
                Debug.Log("Raycast did not hit the player.");
            }
        }
    }

    public void OnSoundHeard(Vector3 location)
    {
        if (isDead) return;

        Debug.Log($"Sound heard at location: {location}");
        soundLocation = location;
        soundHeard = true;
        isReturning = false;
        isChasing = false;
        isAttacking = false;
        isWaiting = false;
        MoveToSoundLocation();
    }

    void MoveToSoundLocation()
    {
        navMeshAgent.SetDestination(soundLocation);
        Debug.Log($"Moving to sound location: {soundLocation}");

        if (Vector3.Distance(transform.position, soundLocation) <= navMeshAgent.stoppingDistance)
        {
            Debug.Log("Reached sound location. Waiting before returning.");
            StartCoroutine(WaitBeforeReturning());
        }
    }

    IEnumerator WaitBeforeReturning()
    {
        isWaiting = true;
        yield return new WaitForSeconds(10f);
        Debug.Log("Finished waiting. Returning to start.");
        isWaiting = false;
        isReturning = true;
        soundHeard = false;
    }

    void ReturnToStart()
    {
        navMeshAgent.SetDestination(startPosition.position);
        Debug.Log($"Returning to start position: {startPosition.position}");

        if (Vector3.Distance(transform.position, startPosition.position) <= navMeshAgent.stoppingDistance)
        {
            Debug.Log("Returned to start position.");
            isReturning = false;
        }
    }

    void ChasePlayer()
    {
        if (player == null) return;

        navMeshAgent.SetDestination(player.position);
        Debug.Log($"Chasing player. Current position: {transform.position}, Player position: {player.position}");

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            Debug.Log("Within attack range. Preparing to attack.");
            navMeshAgent.isStopped = true;
            isChasing = false;
            isAttacking = true;
        }
        else if (distanceToPlayer > detectionRadius * 1.5f)
        {
            Debug.Log("Player out of detection range. Returning to start.");
            isChasing = false;
            isReturning = true;
            navMeshAgent.isStopped = false;
        }
    }

    void AttackPlayer()
    {
        if (Time.time > lastAttackTime + attackCooldown)
        {
            Debug.Log("Attacking player!");

            // Placeholder for actual damage logic
            lastAttackTime = Time.time;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer > attackRange)
            {
                Debug.Log("Player out of attack range. Resuming chase.");
                navMeshAgent.isStopped = false;
                isChasing = true;
                isAttacking = false;
            }
        }
    }

    void UpdateAnimations()
    {
        bool isMoving = navMeshAgent.velocity.magnitude > 0.1f;
        animator.SetBool("isWalking", isMoving);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isDead", isDead);

        if (!isMoving && !isAttacking && !isDead)
        {
            animator.SetBool("isIdle", true);
        }
    }
}
