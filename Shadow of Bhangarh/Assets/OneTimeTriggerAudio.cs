using UnityEngine;

public class OneTimeTriggerAudio : MonoBehaviour
{
    public AudioClip soundToPlay;
    public float volume = 1f;
    public string targetTag = "Player";

    private bool hasPlayed = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed && other.CompareTag(targetTag))
        {
            if (soundToPlay != null)
            {
                audioSource.clip = soundToPlay;
                audioSource.volume = volume;
                audioSource.Play();
                hasPlayed = true;

                // Optional: Destroy the component after playing
                // Destroy(this);
            }
            else
            {
                Debug.LogWarning("No AudioClip assigned to OneTimeTriggerAudio!");
            }
        }
    }
}