using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    Animator doorAnimator;
    public Transform player;
    public float detectionDistance = 3f;
    public LayerMask playerLayer;
    private bool isPlayerNear = false;
    public string keyLayerName = "";
    public List<GameObject> playerKeys; // List to store player's keys

    void Start()
    {
        doorAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckPlayerDistance();
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            OnDoorButtonPress();
        }
    }

    void CheckPlayerDistance()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = player.position - transform.position;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionDistance, playerLayer))
        {
            if (hit.transform == player)
            {
                isPlayerNear = true;
                return;
            }
        }
        isPlayerNear = false;
    }

    public void OnDoorButtonPress()
    {
        OpenDoor();
    }

    void OpenDoor()
    {
        if (doorAnimator != null)
        {
            if (!string.IsNullOrEmpty(keyLayerName))
            {
                bool playerHasKey = false;
                foreach (GameObject key in playerKeys) // Loop through player's keys
                {
                    if (key.layer == LayerMask.NameToLayer(keyLayerName))
                    {
                        playerHasKey = true;
                        break;
                    }
                }

                if (!playerHasKey)
                {
                    return;
                }
            }
            doorAnimator.SetTrigger("Open");
            //sound
            //enemy knows that the door is open
        }
    }
}
