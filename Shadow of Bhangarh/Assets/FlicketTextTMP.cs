using UnityEngine;
using TMPro;

public class FadeInTMPText : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeDelay = 0f;     // How long to wait before starting the fade
    [SerializeField] private float fadeDuration = 1f; // Duration of the fade once it begins

    private TextMeshProUGUI tmpText;
    private float delayTimer;
    private float fadeTimer;

    private void Awake()
    {
        // Get the TextMeshProUGUI component and ensure the text is invisible initially
        tmpText = GetComponent<TextMeshProUGUI>();
        SetAlpha(0f);
    }

    private void OnEnable()
    {
        // Reset timers and alpha when this object is enabled
        delayTimer = 0f;
        fadeTimer = 0f;
        SetAlpha(0f);
    }

    private void Update()
    {
        // If we haven't finished waiting for the delay, just increment delayTimer
        if (delayTimer < fadeDelay)
        {
            delayTimer += Time.deltaTime;
            return;
        }

        // After the delay has passed, start fading in if not already fully faded
        if (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            SetAlpha(alpha);
        }
    }

    /// <summary>
    /// Sets the alpha value of the text color.
    /// </summary>
    /// <param name="alphaValue">Alpha value between 0 and 1.</param>
    private void SetAlpha(float alphaValue)
    {
        Color color = tmpText.color;
        color.a = alphaValue;
        tmpText.color = color;
    }
}
