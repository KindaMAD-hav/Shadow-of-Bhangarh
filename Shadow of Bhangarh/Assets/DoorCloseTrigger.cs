using UnityEngine;

public class DoorCloseTrigger : MonoBehaviour
{
    public Animator doorAnimator; // Reference to the Animator controlling the door
    public string closeTriggerName = "Close"; // Name of the trigger parameter in the Animator

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object stepping on the trigger is the player
        if (other.CompareTag("Player"))
        {
            if (doorAnimator != null)
            {
                // Set the "Close" trigger in the Animator
                doorAnimator.SetTrigger(closeTriggerName);
            }
            else
            {
                Debug.LogWarning("Door Animator is not assigned!");
            }
        }
    }
}
