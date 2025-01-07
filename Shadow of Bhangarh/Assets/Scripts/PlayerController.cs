using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement and Gravity")]
    public float speed = 5f;
    public float gravity = -9.81f;

    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;

    [Header("Camera")]
    public float mouseSenstivity = 2;

    [Header("Jump & Crouch")]
    public float crouchSpeed = 2.5f;
    public float jumpHeight = 1f;
    public float crouchHeight = 0f;
    public float standHeight = 2f;
    public bool isCrouching = false;

    void Start()
    {
        // Initialize the CharacterController
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController component is missing on this GameObject.");
        }
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (characterController == null) return;

        // Check if the player is grounded
        isGrounded = characterController.isGrounded;

        // Reset the velocity when grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // A small downward force to keep the character grounded
        }

        HandleCameraMovement();

        // Handle player movement
        HandlePlayerMovement();

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleJump();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            HandleCrouch();
        }
    }

    void HandlePlayerMovement()
    {
        // Get player input for movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Use crouch speed when crouching
        float currentSpeed = isCrouching ? crouchSpeed : speed;

        // Move the character
        characterController.Move(move * currentSpeed * Time.deltaTime);
    }

    void HandleCameraMovement()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSenstivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSenstivity;
        transform.Rotate(Vector3.up * mouseX);
        float verticalLookRotation = Camera.main.transform.localEulerAngles.x - mouseY;
        Camera.main.transform.localRotation = Quaternion.Euler(verticalLookRotation, 0, 0);
    }

    public void HandleJump()
    {
        if (isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    public void HandleCrouch()
    {
        isCrouching = !isCrouching;
        characterController.height = isCrouching ? crouchHeight : standHeight;
        characterController.radius = isCrouching ? 0.2f : 0.5f;
    }
}
