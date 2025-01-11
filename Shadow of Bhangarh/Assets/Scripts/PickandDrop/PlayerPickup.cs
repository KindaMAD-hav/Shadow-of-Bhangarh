using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public static PlayerPickup instance;
    public float pickupRadius = 2f;
    public KeyCode pickupKey = KeyCode.P;
    public KeyCode dropKey = KeyCode.O;
    public Transform itemHoldPosition;
    public Transform rifleHoldPosition;  // Added specific position for rifles
    public Transform playerBody;
    private GameObject heldItem;
    bool isKey;
    public bool isRifle;

    [Header("Audio")]
    public AudioClip pickupSound;
    public AudioClip dropSound;
    private AudioSource audioSource;

    void Start()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            TryPickupItem();
        }

        if (Input.GetKeyDown(dropKey) && heldItem != null)
        {
            DropItem();
        }

        if (heldItem != null)
        {
            FollowPlayer();
        }
    }

    void TryPickupItem()
    {
        if (heldItem != null)
        {
            Debug.Log("Already holding an item.");
            return;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRadius);
        GameObject closestItem = null;
        float closestDistance = pickupRadius;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("PickupItem") || hitCollider.CompareTag("Rifle"))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestItem = hitCollider.gameObject;
                }
            }
        }

        if (closestItem != null)
        {
            Pickup(closestItem);
        }
    }

    void Pickup(GameObject item)
    {
        heldItem = item;
        heldItem.GetComponent<Collider>().enabled = false;
        Rigidbody itemRb = heldItem.GetComponent<Rigidbody>();
        if (itemRb != null)
        {
            itemRb.isKinematic = true;
        }

        // Set appropriate flags and positions based on item type
        isRifle = item.CompareTag("Rifle");
        isKey = !isRifle && item.CompareTag("PickupItem");

        heldItem.transform.SetParent(playerBody);

        // Use different hold positions for rifles vs regular items
        if (isRifle)
        {
            heldItem.transform.position = rifleHoldPosition.position;
            heldItem.transform.rotation = rifleHoldPosition.rotation;
        }
        else
        {
            heldItem.transform.position = itemHoldPosition.position;
            heldItem.transform.rotation = itemHoldPosition.rotation;
        }

        Debug.Log($"Picked Up: {item.name} ({(isRifle ? "Rifle" : "Regular Item")})");
        PlaySound(pickupSound);
    }

    void DropItem()
    {
        if (heldItem != null)
        {
            heldItem.GetComponent<Collider>().enabled = true;

            Rigidbody itemRb = heldItem.GetComponent<Rigidbody>();
            if (itemRb != null)
            {
                itemRb.isKinematic = false;
            }

            heldItem.transform.SetParent(null);
            heldItem = null;
            isRifle = false;
            isKey = false;
            PlaySound(dropSound);
            VillainAI villainAI = FindObjectOfType<VillainAI>();
            if (villainAI != null)
            {
                villainAI.OnSoundHeard(transform.position);
            }
            Debug.Log("Dropped item.");
        }
    }

    void FollowPlayer()
    {
        if (heldItem != null)
        {
            Transform targetPosition = isRifle ? rifleHoldPosition : itemHoldPosition;
            heldItem.transform.position = targetPosition.position;
            heldItem.transform.rotation = targetPosition.rotation;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}