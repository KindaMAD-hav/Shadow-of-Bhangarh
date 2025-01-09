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


    void Start()
    {
        doorAnimator = GetComponent<Animator>();

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
                Debug.Log("Player reference found and assigned.");
            }
            else
            {
                Debug.LogError("Player GameObject not found! Ensure it has the 'Player' tag.");
            }
        }
    }


    void Update()
    {
        CheckPlayerProximity();
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            OnDoorButtonPress();
        }
    }

    public void OnDoorButtonPress()
    {
        OpenDoor();
    }


    void CheckPlayerProximity()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is not set in the DoorController script!");
            return;
        }
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer <= detectionDistance)
        {
            isPlayerNear = true;
            
        }
        else
        {
            isPlayerNear = false;
        }
    }

    void OpenDoor()
    {
        if (doorAnimator == null)
        {
            Debug.LogError("Animator component is missing!");
            return;
        }

        if (!string.IsNullOrEmpty(keyLayerName))
        {
            bool playerHasKey = false;

            // Check if the player has the required key by checking their inventory or child objects
            foreach (Transform item in player)
            {
                if (item.gameObject.layer == LayerMask.NameToLayer(keyLayerName))
                {
                    playerHasKey = true;
                    Debug.Log("Player has the required key.");
                    break;
                }
            }

            if (!playerHasKey)
            {
                Debug.Log("Player does not have the required key.");
                return;
            }
        }

        doorAnimator.SetTrigger("Open");
        Debug.Log("Door is opening.");
    }

}