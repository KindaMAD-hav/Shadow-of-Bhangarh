using UnityEngine;

public class Chandelier : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private Rigidbody chandelierRigidbody;

    // If you want to do damage or kill the player on impact:
    [Header("Damage")]
    [SerializeField] private int damage = 100; // e.g., 100 could be lethal if the player's max HP is <= 100

    private bool hasDropped = false;

    private void Awake()
    {
        // Ensure the rigidbody is assigned
        if (chandelierRigidbody == null)
            chandelierRigidbody = GetComponent<Rigidbody>();

        // Make sure gravity is disabled at start
        chandelierRigidbody.useGravity = false;
        //chandelierRigidbody.isKinematic = true;
        Debug.Log("rbInitialzed");
    }

    public void Drop()
    {
        if (!hasDropped)
        {
            //chandelierRigidbody.isKinematic = false;
            chandelierRigidbody.useGravity = true;
            hasDropped = true;
            Debug.Log("chanDropped");
        }
    }

    // If the chandelier should deal damage on collision:
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // Attempt to get the player's health component
            //PlayerHealth playerHealth = collision.collider.GetComponent<PlayerHealth>();
            //if (playerHealth != null)
            //{
            //    playerHealth.TakeDamage(damage);
            //}
            Debug.Log("HITT");
        }
    }
}
