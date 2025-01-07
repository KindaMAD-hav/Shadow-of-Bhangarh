using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRadius = 2f;
    public KeyCode pickupKey = KeyCode.P;
    public KeyCode dropKey = KeyCode.O;
    public Transform itemHoldPosition;  // Position where the item should be held
    public Transform playerBody;        // The player's body to attach the item to
    private GameObject heldItem;        // The currently held item
    private bool isKey;                 // Flag for special item types (if needed)

    void Update()
    {
        // Handle pickup and drop logic
        if (Input.GetKeyDown(pickupKey))
        {
            TryPickupItem();
        }

        if (Input.GetKeyDown(dropKey) && heldItem != null)
        {
            DropItem();
        }

        // Update held item's position to follow the player if an item is held
        if (heldItem != null)
        {
            FollowPlayer();
        }
    }

    void TryPickupItem()
    {
        // Prevent picking up a new item if one is already held
        if (heldItem != null)
        {
            Debug.Log("Already holding an item.");
            return;
        }

        // Look for pickable items within the specified radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRadius);
        GameObject closestItem = null;
        float closestDistance = pickupRadius;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("PickupItem"))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestItem = hitCollider.gameObject;
                }
            }
        }

        // If a valid item is found, pick it up
        if (closestItem != null)
        {
            Pickup(closestItem);
        }
    }

    void Pickup(GameObject item)
    {
        heldItem = item;
        heldItem.GetComponent<Collider>().enabled = false;  // Disable collider to prevent unwanted interactions

        Rigidbody itemRb = heldItem.GetComponent<Rigidbody>();
        if (itemRb != null)
        {
            itemRb.isKinematic = true;  // Disable physics interactions for the held item
        }

        isKey = true;  // Set the flag to true if it's a key item (you can extend this later)

        // Attach item to the player and set its initial position
        heldItem.transform.SetParent(playerBody);
        heldItem.transform.position = itemHoldPosition.position;
        heldItem.transform.rotation = itemHoldPosition.rotation;

        Debug.Log("Picked Up: " + item.name);
    }

    void DropItem()
    {
        if (heldItem != null)
        {
            heldItem.GetComponent<Collider>().enabled = true;  // Re-enable collider for interaction

            Rigidbody itemRb = heldItem.GetComponent<Rigidbody>();
            if (itemRb != null)
            {
                itemRb.isKinematic = false;  // Re-enable physics interactions
            }

            heldItem.transform.SetParent(null);  // Detach from player
            heldItem = null;

            Debug.Log("Dropped item.");
        }
    }

    // Update the position of the held item to follow the player's holding position
    void FollowPlayer()
    {
        if (heldItem != null)
        {
            heldItem.transform.position = itemHoldPosition.position;  // Update position
            heldItem.transform.rotation = itemHoldPosition.rotation;  // Update rotation (if necessary)
        }
    }
}
