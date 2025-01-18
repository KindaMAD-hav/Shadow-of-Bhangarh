using UnityEngine;
using System.Collections;

public class HammerSwing : MonoBehaviour
{
    public Transform hammer; // Reference to the hammer transform
    public float swingSpeed = 5f; // Speed of the swing
    public float returnSpeed = 15f; // Speed of returning to initial position
    public float swingAngle = 60f; // Maximum swing angle
    private Quaternion initialRotation; // Hammer's initial rotation
    private bool isSwinging = false;

    void Start()
    {
        if (hammer == null)
        {
            Debug.LogError("Hammer is not assigned!");
            return;
        }
        initialRotation = hammer.localRotation; // Save the initial rotation
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            if (!isSwinging)
                StartCoroutine(SwingHammer());
        }
    }

    private IEnumerator SwingHammer()
    {
        isSwinging = true;

        // Rotate forward
        Quaternion targetRotation = initialRotation * Quaternion.Euler(0, swingAngle, 0);
        float forwardProgress = 0f;

        while (forwardProgress < 1f)
        {
            forwardProgress += Time.deltaTime * swingSpeed;
            hammer.localRotation = Quaternion.Slerp(initialRotation, targetRotation, forwardProgress);
            yield return null;
        }

        // Rotate back to initial position
        float returnProgress = 0f;

        while (returnProgress < 1f)
        {
            returnProgress += Time.deltaTime * returnSpeed;
            hammer.localRotation = Quaternion.Slerp(targetRotation, initialRotation, returnProgress);
            yield return null;
        }

        hammer.localRotation = initialRotation; // Ensure it's exactly reset
        isSwinging = false;
    }
}
