using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]


public class PlayerController : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float rotateSpeed = 0.8f;
    private float gravityValue = -9.81f;

    private Vector3 playerVelocity;
    private bool groundedPlayer;

    private Camera camera;

    private CharacterController characterController;
    private PlayerInput characterInput;

    private Vector2 moveInput;
    private InputAction movementAction;
    private InputAction jumpAction;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        characterInput = GetComponent<PlayerInput>();
        camera = FindObjectOfType<Camera>();
        movementAction = characterInput.actions["Movement"];
        jumpAction = characterInput.actions["Jump"];

    }

    private void OnEnable()
    {
        movementAction.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        jumpAction.performed += ctx => Jump();
        characterInput.actions.Enable();
    }

    private void OnDisable()
    {
        movementAction.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        jumpAction.performed -= ctx => Jump();
        characterInput.actions.Disable();
    }

    void Update()
    {
        groundedPlayer = characterController.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0.0f;
        }

        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        moveDirection = moveDirection.x * camera.transform.right.normalized + moveDirection.z * camera.transform.forward;
        moveDirection.y = 0.0f;
        

        characterController.Move(moveDirection * Time.deltaTime * moveSpeed);


        playerVelocity.y += gravityValue * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);

        
        Quaternion targetRotation = Quaternion.Euler(0, camera.transform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    public void Jump()
    {
        if (groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
    }
}
