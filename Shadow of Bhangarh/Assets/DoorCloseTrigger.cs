using UnityEngine;

public class DoorCloseTrigger : MonoBehaviour
{
    public Animator doorAnimator; // Reference to the Animator controlling the door
    public string closeTriggerName = "Close"; // Name of the trigger parameter in the Animator

    [Header("Audio Settings")]
    public AudioSource audioSource; // Reference to an existing AudioSource
    public AudioClip doorCloseSound; // The audio clip to play when the door closes

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object stepping on the trigger is the player
        if (other.CompareTag("Player"))
        {
            if (doorAnimator != null)
            {
                // Set the "Close" trigger in the Animator
                doorAnimator.SetTrigger(closeTriggerName);

                // Play the door close sound
                if (audioSource != null && doorCloseSound != null)
                {
                    audioSource.PlayOneShot(doorCloseSound);
                }
                else
                {
                    Debug.LogWarning("AudioSource or doorCloseSound is not assigned!");
                }
            }
            else
            {
                Debug.LogWarning("Door Animator is not assigned!");
            }

            // Destroy the trigger after the interaction
            Destroy(gameObject, 0.1f); // Slight delay to ensure sound and animation trigger
        }
    }
}
