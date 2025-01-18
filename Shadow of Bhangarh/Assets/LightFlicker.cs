using UnityEngine;

public class InstantFlicker : MonoBehaviour
{
    // Intensity values for flickering
    public float lightOnIntensity = 3f; // Maximum intensity (e.g., light ON)
    public float lightOffIntensity = 0f; // Minimum intensity (e.g., light OFF)

    // Time range for flicker intervals
    public float minFlickerInterval = 0.05f; // Minimum time between flickers
    public float maxFlickerInterval = 0.2f; // Maximum time between flickers

    // Reference to components
    private Light torchLight;
    private VillainAI villainAI;

    // Timer for controlling flicker
    private float flickerTimer;

    // Track previous chase state to handle transitions
    private bool wasChasing = false;

    void Start()
    {
        // Get the Light component on this GameObject
        torchLight = GetComponent<Light>();
        if (torchLight == null)
        {
            Debug.LogError("No Light component found! Attach this script to a GameObject with a Light component.");
            return;
        }

        // Find the VillainAI component in the scene
        villainAI = FindObjectOfType<VillainAI>();
        if (villainAI == null)
        {
            Debug.LogError("No VillainAI found in the scene! Make sure the villain exists.");
            return;
        }

        // Initialize light to normal state
        torchLight.intensity = lightOnIntensity;
        SetNextFlicker();
    }

    void Update()
    {
        if (torchLight == null || villainAI == null) return;

        bool isChasing = villainAI.isChasing;

        // Handle state transitions
        if (isChasing != wasChasing)
        {
            if (isChasing)
            {
                // Starting to chase - begin flickering
                SetNextFlicker();
            }
            else
            {
                // No longer chasing - reset light to normal
                torchLight.intensity = lightOnIntensity;
            }
            wasChasing = isChasing;
        }

        // Only process flickering when chasing
        if (isChasing)
        {
            // Decrease the timer
            flickerTimer -= Time.deltaTime;

            // If the timer reaches 0, toggle the light and reset the timer
            if (flickerTimer <= 0)
            {
                // Toggle light intensity
                torchLight.intensity = (torchLight.intensity == lightOnIntensity) ? lightOffIntensity : lightOnIntensity;
                // Set the next flicker timer
                SetNextFlicker();
            }
        }
    }

    void SetNextFlicker()
    {
        // Randomize the time interval for the next flicker
        flickerTimer = Random.Range(minFlickerInterval, maxFlickerInterval);
    }
}