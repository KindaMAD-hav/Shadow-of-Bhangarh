using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public static PlayerPickup instance;
    public float pickupRadius = 2f;
    public KeyCode pickupKey = KeyCode.P;
    public KeyCode dropKey = KeyCode.O;
    public Transform itemHoldPosition;
    public Transform rifleHoldPosition;
    public Transform pickAxeHoldPosition;
    public Transform playerBody;

    private GameObject heldItem;
    private bool isKey;
    private bool isRifle;
    private bool isPickAxe;

    // Proper public properties with get/set accessors
    public GameObject CurrentlyHeldItem
    {
        get { return heldItem; }
        private set { heldItem = value; }
    }

    public bool HasKey
    {
        get { return isKey; }
        private set { isKey = value; }
    }

    public bool IsRifle
    {
        get { return isRifle; }
        private set { isRifle = value; }
    }

    public bool IsPickAxe
    {
        get { return isPickAxe; }
        private set { isPickAxe = value; }
    }

    [Header("Audio")]
    public AudioClip pickupSound;
    public AudioClip dropSound;
    public AudioClip breakSound;
    private AudioSource audioSource;

    [Header("Breakable Settings")]
    public LayerMask breakableLayer;

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
            // If it's PICTURE1, disable gravity when picked up
            if (item.name == "PICTURE1")
            {
                itemRb.useGravity = false;
            }
        }

        // Set appropriate flags and positions based on item type
        IsRifle = item.CompareTag("Rifle");
        HasKey = !IsRifle && item.CompareTag("PickupItem");
        IsPickAxe = item.CompareTag("PickAxe");

        heldItem.transform.SetParent(playerBody);

        // Use different hold positions for rifles, pickaxes, and regular items
        if (IsRifle)
        {
            heldItem.transform.position = rifleHoldPosition.position;
            heldItem.transform.rotation = rifleHoldPosition.rotation;
        }
        else if (IsPickAxe)
        {
            heldItem.transform.position = pickAxeHoldPosition.position;
            heldItem.transform.rotation = pickAxeHoldPosition.rotation;
        }
        else
        {
            heldItem.transform.position = itemHoldPosition.position;
            heldItem.transform.rotation = itemHoldPosition.rotation;
        }

        Debug.Log($"Picked Up: {item.name} ({(IsRifle ? "Rifle" : IsPickAxe ? "PickAxe" : "Regular Item")})");
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
                if (heldItem.name == "PICTURE1")
                {
                    itemRb.useGravity = true;
                }
            }

            heldItem.transform.SetParent(null);
            heldItem = null;
            IsRifle = false;
            HasKey = false;
            IsPickAxe = false;
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
            Transform targetPosition = IsRifle ? rifleHoldPosition : (IsPickAxe ? pickAxeHoldPosition : itemHoldPosition);
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

    public void ForceDropItem()
    {
        if (heldItem != null)
        {
            // Destroy the held item
            Destroy(heldItem);

            // Clear the reference and reset item-related flags
            heldItem = null;
            IsRifle = false;
            HasKey = false;
            IsPickAxe = false;

            // Don't play drop sound for forced drops
            // Don't alert the villain for forced drops
        }
    }
}