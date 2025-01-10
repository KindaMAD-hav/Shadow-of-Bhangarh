using UnityEngine;

public class Brick : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] private Vector3 pressedOffset = new Vector3(0, 0, -0.1f);
    [SerializeField] private Vector3 originalLocalPosition;

    [Header("Puzzle Manager Reference")]
    public BrickPuzzleManager puzzleManager;  // Assign in Inspector or via script.

    private bool isPressed = false;

    private void Awake()
    {
        // Store the brick's starting local position so we can return to it.
        originalLocalPosition = transform.localPosition;
    }

    // This method gets called when the player interacts (clicks or uses) this brick.
    public void Interact()
    {
        ToggleBrickState();
    }

    private void ToggleBrickState()
    {
        if (!isPressed)
        {
            // Move it inward by pressedOffset
            transform.localPosition = originalLocalPosition + pressedOffset;
            isPressed = true;
        }
        else
        {
            // Move it back to original position
            transform.localPosition = originalLocalPosition;
            isPressed = false;
        }

        // Let the puzzle manager know something changed
        puzzleManager.CheckPuzzleState();
    }

    // Optional: Provide a way for the manager to check if this brick is pressed.
    public bool IsPressed()
    {
        return isPressed;
    }
}
