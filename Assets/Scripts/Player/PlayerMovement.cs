using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float gravity = -9.81f;

    public bool IsMoving { get; private set; }

    private CharacterController characterController;
    private PlayerControls playerControls;
    private Vector2 moveInput;
    private float verticalVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerControls = new PlayerControls();
        
        // Subscribe to interact event
        playerControls.Player.Interact.performed += OnInteract;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        Interact();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        // Read input from the new Input System
        moveInput = playerControls.Player.Move.ReadValue<Vector2>();
        
        // Calculate movement direction (X and Z)
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        IsMoving = moveDirection.magnitude > 0.1f;

        // Apply gravity
        if (characterController.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Small downward force to stick to ground
        }
        verticalVelocity += gravity * Time.deltaTime;

        // Combine movement and gravity
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = verticalVelocity;

        // Apply to CharacterController
        characterController.Move(velocity * Time.deltaTime);

        // Handle Rotation
        if (IsMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void Interact()
    {
        PlayerInteraction interaction = GetComponent<PlayerInteraction>();
        if (interaction != null)
        {
            interaction.OnInteractPressed();
        }
    }
}
