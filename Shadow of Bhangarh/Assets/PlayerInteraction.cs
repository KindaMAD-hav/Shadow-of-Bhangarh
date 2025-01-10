using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionRange = 3f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // or left-click, etc.
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, interactionRange))
            {
                Brick brick = hit.collider.GetComponent<Brick>();
                if (brick != null)
                {
                    brick.Interact();
                }
            }
        }
    }
}
