using UnityEngine;

public class TripwireTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Chandelier chandelier; // Assign in inspector
    [SerializeField] private bool oneTimeTrigger = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering is the player (by tag or comparing a Player component)
        if (!hasTriggered && other.CompareTag("Player"))
        {
            Debug.Log("triggered");
            // Trigger the chandelier drop
            chandelier.Drop();

            // Prevent re-triggering if you only want it once
            if (oneTimeTrigger)
            {
                hasTriggered = true;
            }
        }
    }
}
