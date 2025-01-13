using UnityEngine;

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

    void Start()
    {
        doorAnimator = GetComponent<Animator>();
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
            // Get reference to PlayerPickup
            PlayerPickup playerPickup = player.GetComponent<PlayerPickup>();
            if (playerPickup != null && playerPickup.HasKey)
            {
                // Drop and destroy the key
                playerPickup.ForceDropItem();

                // Open the door
                doorAnimator.SetTrigger("Open");
                Debug.Log("Door is opening.");
            }
            else
            {
                Debug.Log("Player does not have the required key.");
            }
        }
        else
        {
            // If no key is required, just open the door
            doorAnimator.SetTrigger("Open");
            Debug.Log("Door is opening.");
        }
    }

    bool CheckForKey()
    {
        // Check using the PlayerPickup component
        PlayerPickup playerPickup = player.GetComponent<PlayerPickup>();
        if (playerPickup != null && playerPickup.CurrentlyHeldItem != null)
        {
            if (playerPickup.HasKey && playerPickup.CurrentlyHeldItem.layer == LayerMask.NameToLayer(keyLayerName))
            {
                return true;
            }
        }

        // Fallback check for any key items that might be children of the player
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