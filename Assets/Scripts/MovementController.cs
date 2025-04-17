using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;
    Animator anim;

    [Header("Movement Variables")]
    public float walkSpeed;
    public float runSpeed;
    public float rotationSpeed;

    [Header("Gravity Variables")]
    public float groundedGravity = -0.05f;
    public float gravity = -5f;
    public float fallMultiplier;
    
    [Header("Jumping Variables")]
    bool isJumpPressed = false;
    float jumpVelocity;
    public float maxJumpHeight;
    public float maxJumpTime;
    bool isJumping = false;


    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    Vector3 appliedMovement;
    bool isMovementPressed;
    bool isRunPressed;

    void Awake() 
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        playerInput.Player.Move.performed += onMovementInput;
        playerInput.Player.Move.canceled += onMovementInput;
        playerInput.Player.Run.performed += onRunInput;
        playerInput.Player.Run.canceled += onRunInput;
        playerInput.Player.Jump.started += onJumpInput;
        playerInput.Player.Jump.canceled += onJumpInput;

        SetupJumpVariables();
    }

    void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        jumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void onJumpInput(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    void onRunInput(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }   

    void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x * walkSpeed;
        currentMovement.z = currentMovementInput.y * walkSpeed;
        currentRunMovement.x = currentMovementInput.x * runSpeed;
        currentRunMovement.z = currentMovementInput.y * runSpeed;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void HandleJump()
    {
        if(!isJumping && characterController.isGrounded && isJumpPressed)
        {
            anim.SetBool("isJumping", true);
            isJumping = true;
            currentMovement.y = jumpVelocity;
            appliedMovement.y = jumpVelocity;
        }
        else if(!isJumpPressed && isJumping && characterController.isGrounded)
        {
            isJumping = false;
        }
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0f;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if(isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void HandleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;


        if(characterController.isGrounded)
        {
            anim.SetBool("isJumping", false);
            groundedGravity = -0.05f;
            currentMovement.y = groundedGravity;
            appliedMovement.y = groundedGravity;
        }
        else if(isFalling)
        {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            appliedMovement.y = (previousYVelocity + currentMovement.y) / 2;
        }
        else
        {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * Time.deltaTime);
            appliedMovement.y = (previousYVelocity + currentMovement.y) / 2;
        }
    }

    void HandleAnimation()
    {
        bool isWalking = anim.GetBool("isWalking");
        bool isRunning = anim.GetBool("isRunning");

        if(isMovementPressed && !isWalking)
        {
            anim.SetBool("isWalking", true);
        }
        else if(!isMovementPressed && isWalking)
        {
            anim.SetBool("isWalking", false);
        }

        if((isMovementPressed && isRunPressed) && !isRunning)
        {
            anim.SetBool("isRunning", true);
        }
        else if((!isMovementPressed || !isRunPressed) && isRunning)
        {
            anim.SetBool("isRunning", false);     
        }
    } 
    

    void Update()
    {
        HandleRotation();
        HandleAnimation();
        if(isRunPressed)
        {
            appliedMovement.x = currentRunMovement.x;
            appliedMovement.z = currentRunMovement.z;
        }
        else
        {
            appliedMovement.x = currentMovement.x;
            appliedMovement.z = currentMovement.z;
        }

        characterController.Move(appliedMovement * Time.deltaTime);

        HandleGravity();
        HandleJump();

        Debug.Log(characterController.isGrounded);

    }

    void OnEnable() 
    {   
        playerInput.Player.Enable();        
    }

    void OnDisable() 
    {
        playerInput.Player.Disable();   
    }
}
