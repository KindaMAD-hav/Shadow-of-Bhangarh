using UnityEngine;
using System.Collections;

public class CameraPanToEnemy : MonoBehaviour
{
    public Transform enemyTransform; // Assign the enemy's transform in the Inspector
    public Transform playerCamera;   // Assign the player's camera transform in the Inspector
    public MonoBehaviour playerCameraControlScript; // Reference to your player's camera control script

    [Range(0.1f, 10.0f)] public float panSpeed = 1.0f; // Adjustable speed in the Inspector
    public float focusDuration = 1.5f; // Time to keep the camera focused on the enemy
    public Vector3 targetOffset = new Vector3(0, 1.5f, 0); // Offset for enemy's face (adjust as needed)

    private bool isPanning = false;

    public void OnPlayerHit()
    {
        if (!isPanning)
        {
            StartCoroutine(PanToEnemy());
        }
    }

    private IEnumerator PanToEnemy()
    {
        isPanning = true;

        // Disable player camera control
        playerCameraControlScript.enabled = false;

        // Calculate the target rotation to face the enemy's face
        Quaternion originalRotation = playerCamera.rotation;
        Vector3 targetPosition = enemyTransform.position + targetOffset;
        Vector3 directionToEnemy = (targetPosition - playerCamera.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);

        // Smoothly pan the camera
        float elapsedTime = 0f;
        float duration = 1.0f / panSpeed; // Adjust duration based on pan speed
        while (elapsedTime < duration)
        {
            playerCamera.rotation = Quaternion.Slerp(originalRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        playerCamera.rotation = targetRotation;

        // Hold the focus on the enemy
        yield return new WaitForSeconds(focusDuration);

        // Smoothly return to the original rotation
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            playerCamera.rotation = Quaternion.Slerp(targetRotation, originalRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        playerCamera.rotation = originalRotation;

        // Re-enable player camera control
        playerCameraControlScript.enabled = true;

        isPanning = false;
    }
}
