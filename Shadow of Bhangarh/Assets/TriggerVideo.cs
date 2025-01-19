using UnityEngine;
using UnityEngine.Video;

public class TriggerVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Reference to the VideoPlayer component
    public GameObject videoScreen; // Optional: the object displaying the video (e.g., UI Canvas)
    public GameObject Villain;

    private void Start()
    {
        if (videoScreen != null)
        {
            videoScreen.SetActive(false); // Hide the video screen initially
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player collides with the trigger
        if (other.CompareTag("Player"))
        {
            Villain.SetActive(false);
            if (videoScreen != null)
            {
                videoScreen.SetActive(true); // Show the video screen
            }

            if (videoPlayer != null)
            {
                videoPlayer.Play(); // Start the video
            }
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    // Optionally stop the video when the player exits the trigger
    //    if (other.CompareTag("Player"))
    //    {
    //        if (videoPlayer != null)
    //        {
    //            videoPlayer.Stop(); // Stop the video
    //        }

    //        if (videoScreen != null)
    //        {
    //            videoScreen.SetActive(false); // Hide the video screen
    //        }
    //    }
    //}
}
