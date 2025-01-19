using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using static UnityEngine.Rendering.DebugUI;

public class SceneChanger : MonoBehaviour
{
    // Name of the scene to load
    public string sceneName;
    // Delay in seconds
    public float delay;
    public GameObject controlPanel;

    // Public method to trigger the scene change (for Unity Button)
    private void Awake()
    {
       controlPanel.SetActive(false);
    }
    public void OnClick()
    {
        if (controlPanel != null)
        {
            controlPanel.SetActive(true); // Turn on the panel
        }
        StartCoroutine(LoadSceneAfterDelay());
    }

    // Coroutine to handle the delayed scene load
    private IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(delay); // Wait for the specified time
        SceneManager.LoadScene(sceneName); // Load the specified scene
    }
}
