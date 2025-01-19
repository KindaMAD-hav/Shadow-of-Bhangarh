using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    [Header("Background Music Settings")]
    public AudioClip bgmClip; // Assign your background music clip here
    public AudioSource audioSource; // Optional: Assign an external AudioSource

    [Range(0f, 1f)]
    public float volume = 0.5f; // Adjustable volume for the music

    void Awake()
    {
        // If no external AudioSource is provided, use the one attached to the GameObject
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
            {
                // Automatically add an AudioSource if none exists
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Configure the AudioSource
        audioSource.loop = true; // Loop the music
        audioSource.playOnAwake = false; // Prevent autoplay
        audioSource.volume = volume; // Set initial volume

        // Assign the music clip to the AudioSource
        if (bgmClip != null)
        {
            audioSource.clip = bgmClip;
            audioSource.Play(); // Start playing the music
        }
        else
        {
            Debug.LogWarning("No background music clip assigned!");
        }
    }

    // Optional: Public method to stop music
    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // Optional: Public method to play music
    public void PlayMusic()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    // Optional: Public method to change the volume
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume); // Ensure volume is between 0 and 1
        audioSource.volume = volume;
    }
}
