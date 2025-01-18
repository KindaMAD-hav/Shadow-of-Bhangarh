using UnityEngine;
using UnityEngine.AI;

public class DoorController : MonoBehaviour
{
    Animator doorAnimator;
    public Transform player;
    public float detectionDistance = 3f;
    public LayerMask playerLayer;
    private bool isPlayerNear = false;
    public string keyLayerName = "";
    [Header("Key Position")]
    public Transform keyHoldPosition;

    [Header("NavMesh Settings")]
    public NavMeshObstacle navMeshObstacle;  // Reference to the NavMeshObstacle component
    public float carveDelay = 0.5f;          // Delay before carving the NavMesh (to sync with animation)

    void Start()
    {
        doorAnimator = GetComponent<Animator>();

        // Get or add NavMeshObstacle component
        if (navMeshObstacle == null)
        {
            navMeshObstacle = gameObject.AddComponent<NavMeshObstacle>();
            navMeshObstacle.carving = true;
            navMeshObstacle.size = GetComponent<Collider>().bounds.size;  // Match collider size
            Debug.Log("NavMeshObstacle component added and configured.");
        }

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
                Debug.Log("Player reference found and assigned.");
            }
            else
            {
                Debug.LogError("Player GameObject not found! Ensure it has the 'Player' tag.");
            }
        }
    }

    void Update()
    {
        CheckPlayerProximity();
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            OnDoorButtonPress();
        }
    }

    public void OnDoorButtonPress()
    {
        OpenDoor();
    }

    void CheckPlayerProximity()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is not set in the DoorController script!");
            return;
        }
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        isPlayerNear = distanceToPlayer <= detectionDistance;
    }

    void OpenDoor()
    {
        if (doorAnimator == null)
        {
            Debug.LogError("Animator component is missing!");
            return;
        }

        if (!string.IsNullOrEmpty(keyLayerName))
        {
            PlayerPickup playerPickup = player.GetComponent<PlayerPickup>();
            if (playerPickup != null && playerPickup.HasKey)
            {
                playerPickup.ForceDropItem();
                StartDoorOpenSequence();
            }
            else
            {
                Debug.Log("Player does not have the required key.");
            }
        }
        else
        {
            StartDoorOpenSequence();
        }
    }

    void StartDoorOpenSequence()
    {
        doorAnimator.SetTrigger("Open");
        Debug.Log("Door is opening.");

        // Disable the NavMeshObstacle after a delay to match the animation
        Invoke("DisableNavMeshObstacle", carveDelay);
    }

    void DisableNavMeshObstacle()
    {
        if (navMeshObstacle != null)
        {
            navMeshObstacle.enabled = false;
            Debug.Log("NavMeshObstacle disabled - enemies can now path through the door.");
        }
    }

    // Optional: Add method to close door and re-enable NavMeshObstacle
    public void CloseDoor()
    {
        doorAnimator.SetTrigger("Close");
        if (navMeshObstacle != null)
        {
            navMeshObstacle.enabled = true;
            Debug.Log("Door closed and NavMeshObstacle re-enabled.");
        }
    }

    bool CheckForKey()
    {
        PlayerPickup playerPickup = player.GetComponent<PlayerPickup>();
        if (playerPickup != null && playerPickup.CurrentlyHeldItem != null)
        {
            if (playerPickup.HasKey && playerPickup.CurrentlyHeldItem.layer == LayerMask.NameToLayer(keyLayerName))
            {
                return true;
            }
        }

        foreach (Transform item in player)
        {
            if (item.gameObject.layer == LayerMask.NameToLayer(keyLayerName))
            {
                return true;
            }
        }
        return false;
    }

    public Transform GetKeyHoldPosition()
    {
        return keyHoldPosition;
    }
}