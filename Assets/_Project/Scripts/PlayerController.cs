using UnityEngine;
using Cysharp.Threading.Tasks; // <- potrzebne do UniTask

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeedMultiplier = 2f;
    public float jumpHeight = 1.5f;

    [Header("Look Settings")]
    public Transform cameraHolderTransform;
    public Transform mainCameraTransform;
    public float mouseSensitivity = 100f;
    public float maxLookAngle = 90f;

    [Header("Gravity Settings")]
    public float gravity = -9.81f;
    private Vector3 velocity;

    [Header("Head Bobbing")]
    public float walkBobFrequency = 6f;
    public float walkBobAmplitude = 0.05f;
    public float sprintBobFrequency = 10f;
    public float sprintBobAmplitude = 0.1f;
    private float bobTimer = 0f;
    private Vector3 initialCameraLocalPos;

    [Header("Interactions")]
    public Transform handTransform;
    private InteractionDetection interactionDetector;
    public Interactable interactableInHand;

    // Input
    private PlayerInputActions inputActions;
    private Vector2 inputMove;
    private Vector2 inputLook;
    private bool isSprinting;

    // Rotation
    private float verticalRotation = 0f;

    // Components
    private CharacterController controller;

    // Footstep state
    private bool isPlayingFootsteps = false;


    private void Awake()
    {
        interactionDetector = GetComponent<InteractionDetection>();
        controller = GetComponent<CharacterController>();

        
        initialCameraLocalPos = mainCameraTransform.localPosition;

        InitializeInput();
    }    

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        ReadInput();
        HandleMovement();
        HandleLook();
        ApplyGravity();
        HandleHeadBobbing();

        if (!isPlayingFootsteps && controller.isGrounded && inputMove != Vector2.zero)
        {
            PlayFootstepsLoop().Forget(); // fire and forget
        }
    }

    private void InitializeInput()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Sprint.performed += ctx => isSprinting = true;
        inputActions.Player.Sprint.canceled += ctx => isSprinting = false;

        inputActions.Player.Jump.performed += ctx => TryJump();

        inputActions.Player.Interact.performed += ctx => HandleInteraction();
        inputActions.Player.PickUp.performed += ctx => HandlePickingUpInteractables();

        
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void HandleInteraction()
    {
        Interactable interactable = interactionDetector.CurrentHoveredInteractable;
        if(interactable)
        {            
            interactable.Interact(this);
        }
    }

    private void HandlePickingUpInteractables()
    {
        if (interactableInHand != null)
        {
            TryDropItem();
            return;
        }

        Interactable interactable = interactionDetector.CurrentHoveredInteractable;
        if (interactable != null)
        {
            interactable.PickUp(this);
        }
    }

    private void TryDropItem()
    {
        if (!interactionDetector.IsValidDropSurface)
        {
            Debug.Log("Invalid drop surface!");
            return;
        }

        interactableInHand.Drop(interactionDetector.CurrentSurfaceHit);
        interactableInHand = null;
    }


    private void ReadInput()
    {
        inputMove = inputActions.Player.Move.ReadValue<Vector2>();
        inputLook = inputActions.Player.Look.ReadValue<Vector2>();
    }

    private void HandleMovement()
    {
        float currentSpeed = moveSpeed * (isSprinting ? sprintSpeedMultiplier : 1f);
        Vector3 move = transform.right * inputMove.x + transform.forward * inputMove.y;
        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    private void TryJump()
    {
        if (controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            Debug.Log("siema");
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        float mouseX = inputLook.x * mouseSensitivity * Time.deltaTime;
        float mouseY = inputLook.y * mouseSensitivity * Time.deltaTime;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);

        cameraHolderTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleHeadBobbing()
    {
        if (!controller.isGrounded || inputMove == Vector2.zero)
        {
            bobTimer = 0f;
            mainCameraTransform.localPosition = Vector3.Lerp(
                mainCameraTransform.localPosition,
                initialCameraLocalPos,
                Time.deltaTime * 5f
            );
            return;
        }

        float speedMultiplier = isSprinting ? sprintBobFrequency : walkBobFrequency;
        float amplitude = isSprinting ? sprintBobAmplitude : walkBobAmplitude;

        bobTimer += Time.deltaTime * speedMultiplier;
        float bobOffset = Mathf.Sin(bobTimer) * amplitude;

        Vector3 newPosition = initialCameraLocalPos + new Vector3(0f, bobOffset, 0f);
        mainCameraTransform.localPosition = newPosition;
    }

    private async UniTaskVoid PlayFootstepsLoop()
    {
        isPlayingFootsteps = true;

        while (controller.isGrounded && inputMove != Vector2.zero)
        {
            float delay = isSprinting ? 0.3f : 0.5f;
            float initialDelay = isSprinting ? 0.05f : 0.1f;

            await UniTask.Delay(System.TimeSpan.FromSeconds(initialDelay));

            string clipName = isSprinting ? "playerStepSprint" : "playerStep";
            FindAnyObjectByType<AudioManager>().Play(clipName);

            await UniTask.Delay(System.TimeSpan.FromSeconds(delay));
        }

        isPlayingFootsteps = false;
    }
}
