using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    // Name of the scene to load
    public string sceneName;
    // Delay in seconds
    public float delay;

    // Public method to trigger the scene change (for Unity Button)
    public void OnClick()
    {
        StartCoroutine(LoadSceneAfterDelay());
    }

    // Coroutine to handle the delayed scene load
    private IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(delay); // Wait for the specified time
        SceneManager.LoadScene(sceneName); // Load the specified scene
    }
}
