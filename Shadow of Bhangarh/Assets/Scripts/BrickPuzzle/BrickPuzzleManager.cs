using UnityEngine;

public class BrickPuzzleManager : MonoBehaviour
{
    [Header("Bricks")]
    [SerializeField] private Brick[] bricks; // Assign in Inspector

    [Header("Solution")]
    // This array's length should match the number of bricks (9).
    // true means that brick is *required* to be pressed for the puzzle to be solved
    [SerializeField] private bool[] solution;

    [Header("Door Settings")]
    [SerializeField] private Animator doorAnimator; // Assign the door's Animator in the Inspector
    [SerializeField] private string openTriggerName = "Open"; // Name of the trigger for opening the door

    // A flag to prevent re-triggering the solve event multiple times if you want
    private bool puzzleSolved = false;
    public GameObject outroTrigger;
    public AudioSource doorAudioPlayer;
    public AudioClip doorOpen;
    private void Awake()
    {
        outroTrigger.SetActive(false);
    }
    private void Start()
    {
        // Make sure each Brick references this puzzle manager
        for (int i = 0; i < bricks.Length; i++)
        {
            bricks[i].puzzleManager = this;
        }

        if (doorAnimator == null)
        {
            Debug.LogError("Door Animator is not assigned in the Inspector!");
        }
    }

    public void CheckPuzzleState()
    {
        // Skip checking if it�s already solved (if you don�t want multiple triggers)
        if (puzzleSolved) return;

        // Compare the current brick pressed states to the solution
        bool allMatch = true;
        for (int i = 0; i < bricks.Length; i++)
        {
            bool currentPressed = bricks[i].IsPressed();
            bool requiredPressed = solution[i];

            if (currentPressed != requiredPressed)
            {
                allMatch = false;
                break;
            }
        }

        // If they match, puzzle is solved!
        if (allMatch)
        {
            puzzleSolved = true;
            OnPuzzleSolved();
        }
    }

    private void OnPuzzleSolved()
    {
        Debug.Log("Puzzle solved! Opening the door.");
        outroTrigger.SetActive (true);
        doorAudioPlayer.PlayOneShot(doorOpen);
        // Trigger the door's "Open" animation
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger(openTriggerName);
        }
        else
        {
            Debug.LogError("No door Animator is assigned!");
        }

        // Additional actions (if any)
        // - Enable/disable objects
        // - Trigger other scripts
    }
}