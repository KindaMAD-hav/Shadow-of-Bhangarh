using System.Collections;
using UnityEngine;

public class VillainAI : MonoBehaviour
{
    [Header("Health System")]
    float characterHealth = 100f;
    public float presentHealth;
    public float respawnTime = 5f;

    [Header("Sound Detection")]
    UnityEngine.AI.NavMeshAgent navMeshAgent;
    public float moveSpeed = 3.5f;
    private Vector3 soundLocation;
    public bool isReturning = false;
    public bool isChasing = false;
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

    [Header("Attack Settings")]
    public float attackAnimationDuration = 1.5f;
    public float damageDelay = 0.5f;

    [Header("Navigation")]
    public Transform startPosition;
    public float searchRadius = 2f;

    private Animator animator;

    [Header("Footstep")]
    public AudioClip[] footstepSounds;
    private AudioSource audioSource;
    [Tooltip("Interval between footstep sounds in seconds")]
    public float footstepInterval = 0.5f; // Adjustable in Inspector
    private float nextFootstepTime = 0f;

    void Start()
    {
        Debug.Log("VillainAI initialized.");
        presentHealth = characterHealth;
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (startPosition == null)
        {
            GameObject startMarker = new GameObject($"{gameObject.name}_StartPosition");
            startMarker.transform.position = transform.position;
            startPosition = startMarker.transform;
        }

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

        // Don't look for player while attacking
        if (!isAttacking)
        {
            LookForPlayer();
        }

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
            LookForPlayer();
        }
        else if (isReturning)
        {
            ReturnToStart();
        }
        else if (soundHeard)
        {
            MoveToSoundLocation();
        }

        UpdateAnimations();
        PlayFootstepSounds();
    }

    void LookForPlayer()
    {
        if (player == null) return;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRadius)
        {
            RaycastHit hit;
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRadius))
            {
                if (hit.transform == player)
                {
                    isChasing = true;
                    isReturning = false;
                    isWaiting = false;
                    navMeshAgent.isStopped = false;
                }
            }
        }
    }

    public void OnSoundHeard(Vector3 location)
    {
        if (isDead || isAttacking) return;
        soundLocation = location;
        soundHeard = true;
        isReturning = false;
        isChasing = false;
        isWaiting = false;
    }

    void MoveToSoundLocation()
    {
        navMeshAgent.SetDestination(soundLocation);
        if (Vector3.Distance(transform.position, soundLocation) <= (navMeshAgent.stoppingDistance + 0.1f))
        {
            Debug.Log("Reached sound location");
            soundHeard = false;
            navMeshAgent.isStopped = true;
            CheckForItems();
        }
    }

    void CheckForItems()
    {
        Debug.Log("Checking for items at sound location...");
        bool itemFound = false;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("PickupItem") || hitCollider.CompareTag("Rifle"))
            {
                Debug.Log($"Found item: {hitCollider.gameObject.name}");
                itemFound = true;
                break;
            }
        }

        StartCoroutine(WaitBeforeReturning());
    }

    IEnumerator WaitBeforeReturning()
    {
        isWaiting = true;
        yield return new WaitForSeconds(10f);
        navMeshAgent.isStopped = false;
        isWaiting = false;
        isReturning = true;
    }

    void ReturnToStart()
    {
        navMeshAgent.SetDestination(startPosition.position);
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

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            Debug.Log("Within attack range. Preparing to attack.");
            navMeshAgent.isStopped = true;
            isChasing = false;
            isAttacking = true;
            lastAttackTime = Time.time - attackCooldown; // Allow immediate first attack
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
        // First check if we're actually in range before attempting to attack
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            // If we're out of range, stop attacking and resume chase
            Debug.Log("Player out of attack range. Resuming chase.");
            navMeshAgent.isStopped = false;
            isChasing = true;
            isAttacking = false;
            animator.SetBool("isAttacking", false);
            return;
        }

        // Only start a new attack if we're not already in an attack animation and cooldown is ready
        if (!animator.GetBool("isAttacking") && Time.time > lastAttackTime + attackCooldown)
        {
            StartCoroutine(PerformAttack());
            lastAttackTime = Time.time;
        }
    }

    IEnumerator PerformAttack()
    {
        // Start attack animation
        animator.SetBool("isAttacking", true);

        // Wait for the damage point in the animation
        yield return new WaitForSeconds(damageDelay);

        // Apply damage if player is still in range
        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                Debug.Log("Attacking player!");
                playerController.TakeDamage(100f);

                // Only start respawn if we successfully hit the player
                StartCoroutine(Respawn(respawnTime));
            }
        }

        // Wait for the rest of the animation to complete
        yield return new WaitForSeconds(attackAnimationDuration - damageDelay);

        // Reset attack state
        animator.SetBool("isAttacking", false);

        // Check if we should continue attacking or return to chase
        float finalDistance = Vector3.Distance(transform.position, player.position);
        if (finalDistance > attackRange)
        {
            isChasing = true;
            isAttacking = false;
            navMeshAgent.isStopped = false;
        }
    }

    void UpdateAnimations()
    {
        bool isMoving = navMeshAgent.velocity.magnitude > 0.1f;
        animator.SetBool("isWalking", isMoving);

        if (!isMoving && !isAttacking && !isDead)
        {
            animator.SetBool("isIdle", true);
        }
        else
        {
            animator.SetBool("isIdle", false);
        }
    }

    void PlayFootstepSounds()
    {
        if (navMeshAgent.velocity.magnitude > 0.1f && Time.time >= nextFootstepTime)
        {
            if (footstepSounds.Length > 0)
            {
                AudioClip footstepSound = footstepSounds[Random.Range(0, footstepSounds.Length)];
                audioSource.PlayOneShot(footstepSound);
                nextFootstepTime = Time.time + footstepInterval; 
            }
        }
    }

    public void characterHitDamage(float takeDamage)
    {
        if (isDead) return;
        presentHealth -= takeDamage;
        if (presentHealth <= 0)
        {
            characterDie();
        }
    }

    void characterDie()
    {
        isDead = true;
        moveSpeed = 0f;
        navMeshAgent.speed = moveSpeed;
        detectionRadius = 0f;
        animator.SetBool("isDead", true);
        GetComponent<Collider>().enabled = false;
        navMeshAgent.enabled = false;

        StartCoroutine(Respawn(respawnTime));
    }

    IEnumerator Respawn(float delay)
    {
        yield return new WaitForSeconds(delay);

        presentHealth = characterHealth;
        isDead = false;
        animator.SetBool("isDead", false);
        GetComponent<Collider>().enabled = true;
        navMeshAgent.enabled = true;
        this.enabled = true;

        transform.position = startPosition.position;
        navMeshAgent.Warp(startPosition.position);

        isReturning = false;
        isChasing = false;
        isAttacking = false;
        isWaiting = false;
        soundHeard = false;
        moveSpeed = 3.5f;
        navMeshAgent.speed = moveSpeed;
        detectionRadius = 15f;
    }

    void OnDrawGizmos()
    {
        if (navMeshAgent == null || !navMeshAgent.isActiveAndEnabled || !navMeshAgent.hasPath)
            return;

        Gizmos.color = Color.red;
        Vector3[] pathCorners = navMeshAgent.path.corners;
        for (int i = 0; i < pathCorners.Length - 1; i++)
        {
            Gizmos.DrawLine(pathCorners[i], pathCorners[i + 1]);
        }
    }
}