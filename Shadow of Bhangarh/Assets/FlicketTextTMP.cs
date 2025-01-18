using UnityEngine;
using TMPro;
using System.Collections;

public class JitteryFlickerTMP : MonoBehaviour
{
    [Header("Normal State")]
    [Range(0f, 1f)]
    public float normalAlpha = 1f;
    // The usual alpha (opacity) the text sits at between flickers

    [Header("Flicker State (Range)")]
    [Range(0f, 1f)]
    public float flickerAlphaMin = 0f;
    [Range(0f, 1f)]
    public float flickerAlphaMax = 0.5f;
    // During flicker, alpha will randomly bounce between these values

    [Header("Timings (Seconds)")]
    [Tooltip("Random time (seconds) to wait before flicker starts again.")]
    public float minWaitTime = 0.5f;
    public float maxWaitTime = 2f;

    [Tooltip("Number of quick 'jitter' flickers that happen once flicker begins.")]
    public int minFlickerCount = 2;
    public int maxFlickerCount = 5;

    [Tooltip("Random flicker step duration.")]
    public float minFlickerStepDuration = 0.05f;
    public float maxFlickerStepDuration = 0.15f;

    private TextMeshProUGUI tmpText;
    private Coroutine flickerRoutine;

    private void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        // Start the flicker coroutine
        flickerRoutine = StartCoroutine(FlickerCycle());
    }

    private void OnDisable()
    {
        if (flickerRoutine != null)
            StopCoroutine(flickerRoutine);
    }

    private IEnumerator FlickerCycle()
    {
        // Loop indefinitely
        while (true)
        {
            // 1. Start in normal state
            SetAlpha(normalAlpha);

            // 2. Wait a random time before starting the jitter flicker
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            // 3. Flicker multiple times (jitter)
            int flickerCount = Random.Range(minFlickerCount, maxFlickerCount + 1);
            for (int i = 0; i < flickerCount; i++)
            {
                // a) Pick a random alpha in the flicker range
                float randomAlpha = Random.Range(flickerAlphaMin, flickerAlphaMax);
                SetAlpha(randomAlpha);

                // b) Wait a short, random time for this flicker step
                float flickerStepDuration = Random.Range(minFlickerStepDuration, maxFlickerStepDuration);
                yield return new WaitForSeconds(flickerStepDuration);

                // c) Optionally bounce back towards normalAlpha or do another random alpha
                //    Let's do another random alpha in the flicker range for more chaos
                float bounceAlpha = Random.Range(flickerAlphaMin, flickerAlphaMax);
                SetAlpha(bounceAlpha);

                // d) Wait another short, random time
                flickerStepDuration = Random.Range(minFlickerStepDuration, maxFlickerStepDuration);
                yield return new WaitForSeconds(flickerStepDuration);
            }

            // 4. End flicker cycle by returning to normal alpha
            SetAlpha(normalAlpha);
        }
    }

    private void SetAlpha(float alphaValue)
    {
        Color c = tmpText.color;
        c.a = alphaValue;
        tmpText.color = c;
    }
}
