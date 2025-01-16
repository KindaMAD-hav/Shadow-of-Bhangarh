using UnityEngine;

public class HammerManager : MonoBehaviour
{
    [Header("Assign your hammer GameObjects here")]
    public GameObject OriginalHammer;
    public GameObject BrokenHammer;

    void Start()
    {
        // Ensure the original hammer is active, and the broken hammer is inactive at the start
        if (OriginalHammer != null)
            OriginalHammer.SetActive(true);

        if (BrokenHammer != null)
            BrokenHammer.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            BreakHammer();
        }
    }

    // This method will disable the original hammer and enable the broken hammer
    public void BreakHammer()
    {
        if (OriginalHammer != null && BrokenHammer != null)
        {
            OriginalHammer.SetActive(false);
            BrokenHammer.SetActive(true);
        }
    }

    // Optional: If you want the ability to switch back to the original hammer
    public void RepairHammer()
    {
        if (OriginalHammer != null && BrokenHammer != null)
        {
            OriginalHammer.SetActive(true);
            BrokenHammer.SetActive(false);
        }
    }
}
