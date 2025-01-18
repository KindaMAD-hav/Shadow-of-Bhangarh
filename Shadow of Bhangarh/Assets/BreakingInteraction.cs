using UnityEngine;

public class BreakingInteraction : MonoBehaviour
{
    // Distance within which we can interact (adjust as needed)
    public float interactDistance = 5f;

    // Replace this with your real check (e.g., checking an inventory or equipment system)
    private bool IsHoldingWarhammer()
    {
        if (PlayerPickup.instance != null)
        {
            return PlayerPickup.instance.IsPickAxe;
        }
        return false;
    }

    void Update()
    {
        // Check for left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("clicked");
            // Proceed only if the player is holding the specific warhammer
            if (IsHoldingWarhammer())
            {
                Debug.Log("true");

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, interactDistance))
                {
                    Debug.Log("ray");

                    // Look for a HammerManager on the object hit by the ray
                    HammerManager hammerManager = hit.collider.GetComponent<HammerManager>();
                    if (hammerManager != null)
                    {
                        Debug.Log("hammeredRay");
                        // Break the hammer if we found a HammerManager
                        hammerManager.BreakHammer();
                    }
                }
            }
        }
    }

}
