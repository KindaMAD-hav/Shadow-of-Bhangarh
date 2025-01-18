using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageSequenceWithFade : MonoBehaviour
{
    [SerializeField] private Image[] images;  // The images to fade in
    [SerializeField] private float fadeDuration = 1f;  // Duration of the fade
    [SerializeField] private float delayBetweenImages = 0.5f;  // Delay between each image's appearance

    private void Start()
    {
        StartCoroutine(ShowAndFadeImages());
    }

    private IEnumerator ShowAndFadeImages()
    {
        foreach (Image img in images)
        {
            img.gameObject.SetActive(true);  // Activate the image (show it)
            yield return StartCoroutine(FadeIn(img));  // Fade the image in
            yield return new WaitForSeconds(delayBetweenImages);  // Wait for the delay before showing the next image
        }
    }

    private IEnumerator FadeIn(Image img)
    {
        float elapsedTime = 0f;
        Color startColor = img.color;
        startColor.a = 0f;  // Set the starting alpha to 0 (fully transparent)
        img.color = startColor;  // Apply the starting color

        // Gradually increase the alpha value from 0 to 1 over time
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            img.color = new Color(startColor.r, startColor.g, startColor.b, alpha);  // Update the image color with new alpha
            yield return null;  // Wait for the next frame
        }

        // Ensure the final alpha value is set to 1 (fully visible)
        img.color = new Color(startColor.r, startColor.g, startColor.b, 1f);
    }
}