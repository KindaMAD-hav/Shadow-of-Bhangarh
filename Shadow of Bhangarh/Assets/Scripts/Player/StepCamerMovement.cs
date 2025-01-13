using UnityEngine;

public class StepCameraMovement : MonoBehaviour
{
    [Header("Step Movement Settings")]
    public float stepAmplitude = 0.1f; // How far the camera moves left and right
    public float stepFrequency = 5f; // How fast the steps happen
    public Transform cameraTransform; // The camera to apply the effect to

    private bool isMoving = false;
    private float stepTimer = 0f;
    private Vector3 originalPosition;

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        originalPosition = cameraTransform.localPosition;
    }

    void Update()
    {
        // Check if the player is moving (you can replace this with your own movement logic)
        isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        if (isMoving)
        {
            // Update the step timer
            stepTimer += Time.deltaTime * stepFrequency;

            // Apply step movement using a sine wave
            float stepOffset = Mathf.Sin(stepTimer) * stepAmplitude;
            cameraTransform.localPosition = originalPosition + new Vector3(stepOffset, 0, 0);
        }
        else
        {
            // Reset camera position when not moving
            stepTimer = 0f;
            cameraTransform.localPosition = originalPosition;
        }
    }
}
