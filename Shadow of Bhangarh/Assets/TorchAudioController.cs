using UnityEngine;

public class TorchAudioController : MonoBehaviour
{
    private AudioSource fireSound;

    private void Start()
    {
        // Get the AudioSource component
        fireSound = GetComponent<AudioSource>();
        if (fireSound == null)
        {
            Debug.LogWarning("No AudioSource component found on the torch.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the trigger zone
        if (other.CompareTag("Player") && fireSound != null)
        {
            fireSound.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Stop the sound when the player leaves the trigger zone
        if (other.CompareTag("Player") && fireSound != null)
        {
            fireSound.Stop();
        }
    }
}
