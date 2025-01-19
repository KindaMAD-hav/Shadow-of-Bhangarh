using UnityEngine;

public class QuitButton : MonoBehaviour
{
    // Call this function when the quit button is clicked
    public void QuitGame()
    {
        // Logs to the console for testing purposes (does not appear in the final build)
        Debug.Log("Quit button clicked. Exiting game...");

        // Quit the application
        Application.Quit();
    }
}