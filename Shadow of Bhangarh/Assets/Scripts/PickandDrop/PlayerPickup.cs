using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public static PlayerPickup instance;
    public float pickupRadius = 2f;
    public KeyCode pickupKey = KeyCode.P;
    public KeyCode dropKey = KeyCode.O;
    public Transform itemHoldPosition;
    public Transform rifleHoldPosition;  // Added specific position for rifles
    public Transform pickAxeHoldPosition; // Added specific position for pickaxe
    public Transform playerBody;
    private GameObject heldItem;
    bool isKey;
    public bool isRifle;
    public bool isPickAxe;

    [Header("Audio")]
    public AudioClip pickupSound;
    public AudioClip dropSound;
    public AudioClip breakSound; // Added break sound
    private AudioSource audioSource;

    [Header("Breakable Settings")]
    public LayerMask breakableLayer; // Layer for breakable objects

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

        if (Input.GetMouseButtonDown(0) && isPickAxe)
        {
            TryBreakObject();
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
            if (hitCollider.CompareTag("PickupItem") || hitCollider.CompareTag("Rifle") || hitCollider.CompareTag("PickAxe"))
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
        isPickAxe = item.CompareTag("PickAxe");

        heldItem.transform.SetParent(playerBody);

        // Use different hold positions for rifles, pickaxes, and regular items
        if (isRifle)
        {
            heldItem.transform.position = rifleHoldPosition.position;
            heldItem.transform.rotation = rifleHoldPosition.rotation;
        }
        else if (isPickAxe)
        {
            heldItem.transform.position = pickAxeHoldPosition.position;
            heldItem.transform.rotation = pickAxeHoldPosition.rotation;
        }
        else
        {
            heldItem.transform.position = itemHoldPosition.position;
            heldItem.transform.rotation = itemHoldPosition.rotation;
        }

        Debug.Log($"Picked Up: {item.name} ({(isRifle ? "Rifle" : isPickAxe ? "PickAxe" : "Regular Item")})");
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
            isPickAxe = false;
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
            Transform targetPosition = isRifle ? rifleHoldPosition : (isPickAxe ? pickAxeHoldPosition : itemHoldPosition);
            heldItem.transform.position = targetPosition.position;
            heldItem.transform.rotation = targetPosition.rotation;
        }
    }

    void TryBreakObject()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRadius, breakableLayer))
        {
            if (hit.collider.CompareTag("Breakable"))
            {
                Destroy(hit.collider.gameObject);
                PlaySound(breakSound);
                Debug.Log("Breakable object destroyed.");
            }
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
