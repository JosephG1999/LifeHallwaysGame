using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public Camera playerCamera;
    [SerializeField] float walkSpeed = 6f, runSpeed = 12f, jumpPower = 7f, gravity = 10f;
    [SerializeField] float lookSpeed;
    private float lookXLimit = 90f;


    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    public bool canMove = true;

    [SerializeField] float maxClickDistance; // 10,000 seems good?
    [SerializeField] string moveForward, moveBackward, moveLeft, moveRight;


    CharacterController characterController;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {

        // Handles Movment
        playerMovement();

        // Handles Look Rotation
        characterController.Move(moveDirection * Time.deltaTime);
        playerHeadRotation();

        // Handles Interaction
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) fireRay();
    }

    // Handles Interaction
    void fireRay()
    {
        Ray camRay = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(camRay, out RaycastHit hit, maxClickDistance))
        {
            Debug.Log("RAYCAST HIT!");
            if (hit.transform.CompareTag("DoorUI")) Debug.Log("DOOR HIT!");
        }
    }

    // Movement Handler
    void playerMovement() 
    {
        // WASD movement
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        // Jumping
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
    }

    // Player Look Rotation Handler
    void playerHeadRotation() 
    {
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}
