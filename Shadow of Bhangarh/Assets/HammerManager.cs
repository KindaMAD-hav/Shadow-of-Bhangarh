using UnityEngine;
using UnityEngine.Audio;

public class HammerManager : MonoBehaviour
{
    [Header("Assign your hammer GameObjects here")]
    public GameObject OriginalHammer;
    public GameObject BrokenHammer;
    public AudioClip plankBreak;
    public AudioSource plankAudio;

    void Start()
    {
        // Ensure the original hammer is active, and the broken hammer is inactive at the start
        if (OriginalHammer != null)
        {
            OriginalHammer.SetActive(true);
            Debug.Log("Original Hammer is active.");
        }
        else
        {
            Debug.LogWarning("Original Hammer is not assigned!");
        }

        if (BrokenHammer != null)
        {
            BrokenHammer.SetActive(false);
            Debug.Log("Broken Hammer is inactive.");
        }
        else
        {
            Debug.LogWarning("Broken Hammer is not assigned!");
        }
    }

    // This method will disable the original hammer and enable the broken hammer
    public void BreakHammer()
    {
        // Debugging audio
        if (plankAudio != null)
        {
            Debug.Log("AudioSource is assigned.");
            if (plankBreak != null)
            {
                Debug.Log("AudioClip (plankBreak) is assigned.");
                plankAudio.PlayOneShot(plankBreak); // Play the plank break sound
                Debug.Log("Playing plank break sound.");
            }
            else
            {
                Debug.LogWarning("plankBreak AudioClip is not assigned!");
            }
        }
        else
        {
            Debug.LogWarning("AudioSource (plankAudio) is not assigned!");
        }

        // Debugging hammer switching
        if (OriginalHammer != null && BrokenHammer != null)
        {
            OriginalHammer.SetActive(false); // Deactivate the original hammer
            BrokenHammer.SetActive(true);    // Activate the broken hammer
            Debug.Log("Original Hammer deactivated. Broken Hammer activated.");
        }
        else
        {
            Debug.LogWarning("One of the hammer GameObjects is not assigned!");
        }
    }

    // Optional: If you want the ability to switch back to the original hammer
    public void RepairHammer()
    {
        if (OriginalHammer != null && BrokenHammer != null)
        {
            OriginalHammer.SetActive(true);  // Activate the original hammer
            BrokenHammer.SetActive(false);   // Deactivate the broken hammer
            Debug.Log("Original Hammer activated. Broken Hammer deactivated.");
        }
        else
        {
            Debug.LogWarning("One of the hammer GameObjects is not assigned!");
        }
    }
}
