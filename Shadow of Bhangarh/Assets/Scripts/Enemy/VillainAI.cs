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
    public float searchRadius = 2f; // Radius to check for items at sound location

    private Animator animator;

    [Header("Footstep")]
    public AudioClip[] footstepSounds;
    AudioSource audioSource;
    float footstepInterval = 0.5f;
    float nextFootstepTime = 0f;


    void Start()
    {
        Debug.Log("VillainAI initialized.");
        presentHealth = characterHealth;
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();


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
            else
            {
                Debug.Log("Raycast did not hit the player.");
            }
        }
    }

    public void OnSoundHeard(Vector3 location)
    {
        if (isDead) return;
        soundLocation = location;
        soundHeard = true;
        isReturning = false;
        isChasing = false;
        isAttacking = false;
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

        // Check for any pickup items or rifles in the area
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

        if (!itemFound)
        {
            Debug.Log("No items found at sound location. Starting return timer.");
            StartCoroutine(WaitBeforeReturning());
        }
        else
        {
            Debug.Log("Items found at sound location. Continuing to wait and search.");
            StartCoroutine(WaitBeforeReturning());
        }
    }

    IEnumerator WaitBeforeReturning()
    {
        isWaiting = true;
        yield return new WaitForSeconds(10f);
        navMeshAgent.isStopped = false; // Re-enable movement
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
            

            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                Debug.Log("Attacking player!");
                playerController.TakeDamage(100f);
            }

            lastAttackTime = Time.time;
            StartCoroutine(Respawn(2));

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
        if(presentHealth <= 0)
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

        //UI

        //Respawn
        StartCoroutine(Respawn(respawnTime));
    }
    IEnumerator Respawn(float delay)
    {
        yield return new WaitForSeconds(delay - 3f);
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

        // Set the color for the Gizmos
        Gizmos.color = Color.red;

        // Get the corners of the path
        Vector3[] pathCorners = navMeshAgent.path.corners;

        // Draw the path
        for (int i = 0; i < pathCorners.Length - 1; i++)
        {
            Gizmos.DrawLine(pathCorners[i], pathCorners[i + 1]);
        }
    }

}