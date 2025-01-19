using UnityEngine;
using UnityEngine.Playables;

public class TriggerCutscene : MonoBehaviour
{
    public PlayableDirector playableDirector; // Reference to the PlayableDirector
    public Camera mainCamera; // Reference to the main camera (the one that follows the player)
    public Camera cutsceneCamera; // Reference to the cutscene camera
    public GameObject player; // Reference to the player GameObject (for disabling player control)

    private void Start()
    {
        // Ensure that both cameras are assigned
        if (mainCamera == null || cutsceneCamera == null)
        {
            Debug.LogError("Cameras are not assigned in the Inspector!");
        }

        if (playableDirector == null)
        {
            Debug.LogError("PlayableDirector is not assigned in the Inspector!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            StartCutscene();
        }
    }

    private void StartCutscene()
    {
        if (playableDirector != null)
        {
            // Disable the main camera and player control
            if (mainCamera != null)
            {
                mainCamera.enabled = false; // Disable the main camera
                Debug.Log("Main camera disabled");
            }

            // Enable the cutscene camera
            if (cutsceneCamera != null)
            {
                cutsceneCamera.enabled = true; // Enable the cutscene camera
                Debug.Log("Cutscene camera enabled");
            }

            // Disable player control during the cutscene
            if (player != null)
            {
                player.GetComponent<PlayerController>().enabled = false;
                Debug.Log("Player control disabled");
            }

            // Play the Timeline (cutscene)
            playableDirector.Play();

            // Hook up event to re-enable controls after the cutscene finishes
            playableDirector.stopped += OnCutsceneEnd;
        }
        else
        {
            Debug.LogWarning("PlayableDirector is not assigned!");
        }
    }

    private void OnCutsceneEnd(PlayableDirector director)
    {
        // Re-enable the main camera and player control after the cutscene ends
        if (mainCamera != null)
        {
            mainCamera.enabled = true; // Enable the main camera
            Debug.Log("Main camera enabled");
        }

        if (cutsceneCamera != null)
        {
            cutsceneCamera.enabled = false; // Disable the cutscene camera
            Debug.Log("Cutscene camera disabled");
        }

        if (player != null)
        {
            player.GetComponent<PlayerController>().enabled = true; // Re-enable player control
            Debug.Log("Player control enabled");
        }
    }
}