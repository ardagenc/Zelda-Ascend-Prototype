using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Cinemachine;

public class PlayerStateMachine : MonoBehaviour
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
    public float gravity = -9f;
    public float fallMultiplier = 2f;
    
    [Header("Jumping Variables")]
    bool isJumpPressed = false;
    float jumpVelocity;
    public float maxJumpHeight;
    public float maxJumpTime;
    bool isJumping = false;
    bool requireNewJumpPress = false;

    [Header("Ascend Mechanic Prototype")]
    public RaycastHit hit;
    public GameObject thirdPersonCam;
    public CinemachineFreeLook cam;
    public Collider ascendCollider;
    public GameObject ascendIndicator;
    public Material indicatorMaterial;
    public bool isAscendUsed;
    public bool isAscendPressed;
    public bool climbButtonPressed;
    public float ascendSpeed = 2;
    public bool stopAscend = false;
    public bool switchGroundState = false;
    public AscendVFX ascendVFX;
    public GameObject ascendParticle;
    public GameObject ascendSpiralParticle;
    public GameObject ascendSpiralParticleFlipped;



    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    Vector3 appliedMovement;
    bool isMovementPressed;
    bool isRunPressed;

    Vector3 cameraRelativeMove;

    PlayerBaseState currentState;
    PlayerStateFactory states;

    public PlayerBaseState CurrentState { get { return currentState; } set { currentState = value; } }
    public Animator Anim {get{return anim;}}
    public CharacterController CharacterController {get {return characterController;}}
    public bool IsJumping {get{return isJumping;} set { isJumping = value;}}
    public bool RequireNewJumpPress { get { return requireNewJumpPress; } set { requireNewJumpPress = value; } }
    public bool IsJumpPressed { get { return isJumpPressed; } }
    public float JumpVelocity { get {return jumpVelocity;}} 
    public bool IsMovementPressed { get { return isMovementPressed; } set { isMovementPressed = value; } }
    public bool IsRunPressed { get { return isRunPressed; } set { isRunPressed = value; } }
    public float CurrentMovementY {get {return currentMovement.y;} set { currentMovement.y = value;}}
    public float AppliedMovementY { get {return appliedMovement.y;} set { appliedMovement.y = value;}}
    public float GroundedGravity {get {return groundedGravity;} set { groundedGravity = value;}}
    public float Gravity {get {return gravity;} set { gravity = value; }}
    public float FallMultiplier { get { return fallMultiplier; } set { fallMultiplier = value; } }
    public float AppliedMovementX { get { return appliedMovement.x; } set { appliedMovement.x = value; } }
    public float AppliedMovementZ { get { return appliedMovement.z; } set { appliedMovement.z = value; } }
    public Vector2 CurrentMovementInput { get { return currentMovementInput;} }

    void Awake() 
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        //setup state
        states = new PlayerStateFactory(this);

        currentState = states.Grounded();
        currentState.EnterState();

        playerInput.Player.Move.performed += onMovementInput;
        playerInput.Player.Move.canceled += onMovementInput;
        playerInput.Player.Run.performed += onRunInput;
        playerInput.Player.Run.canceled += onRunInput;
        playerInput.Player.Jump.started += onJumpInput;
        playerInput.Player.Jump.canceled += onJumpInput;
        playerInput.Player.Ascend.started += onAscendInput;
        //playerInput.Player.Ascend.canceled += onAscendInput;
        playerInput.Player.AscendConfirm.started += doAscend;
        playerInput.Player.AscendConfirm.canceled += doAscend;
        playerInput.Player.AscendClimb.started += ascendClimbInput;

        SetupJumpVariables();
    }

    void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        jumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void onAscendInput(InputAction.CallbackContext context)
    {  
        if(isAscendUsed)    
        {
            isAscendUsed = false;
        }
        else
        {
            isAscendUsed = true;
        }
    }

    void doAscend(InputAction.CallbackContext context)
    {
        if(currentState.ToString() == "PlayerAscendState" && hit.point.y - transform.position.y < 15 && hit.point != Vector3.zero)
        {
            isAscendPressed = true;
        }
    }

    void ascendClimbInput(InputAction.CallbackContext context)
    {
        if(currentState.ToString() == "PlayerAscendState" && stopAscend)
        {
            climbButtonPressed = context.ReadValueAsButton(); 
        }
    }

    void onJumpInput(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
        requireNewJumpPress = false;
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

    void OnEnable() 
    {   
        playerInput.Player.Enable();        
    }

    void OnDisable() 
    {
        playerInput.Player.Disable();   
    }

    void Update()
    {
        HandleRotation();
        currentState.UpdateStates();

        cameraRelativeMove = ConvertToCameraSpace(appliedMovement);
        characterController.Move(cameraRelativeMove * Time.deltaTime);

        Physics.Raycast(transform.position + Vector3.up, Vector3.up, out hit);

        if(isAscendUsed)
        {
            AscendIndicator();
            MaterialColorChange();
        }
        else
        {
            ascendIndicator.SetActive(false);
            cam.m_Priority = 10;
        }

        Debug.Log(climbButtonPressed);
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = cameraRelativeMove.x;
        positionToLookAt.y = 0f;
        positionToLookAt.z = cameraRelativeMove.z;

        Quaternion currentRotation = transform.rotation;

        if(isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    Vector3 ConvertToCameraSpace(Vector3 vectorToRotate)
    {
        float currentYValue = vectorToRotate.y;

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;

        Vector3 cameraForwardZProduct = vectorToRotate.z * cameraForward;
        Vector3 cameraRightXProduct = vectorToRotate.x * cameraRight;

        Vector3 vectorRotatedToCameraSpace = cameraForwardZProduct + cameraRightXProduct;
        vectorRotatedToCameraSpace.y = currentYValue;
        return vectorRotatedToCameraSpace;
    }

    public void AscendIndicator()
    {
        ascendIndicator.transform.position = hit.point;
        
        if(hit.collider == null || isAscendPressed)
        {
            ascendIndicator.SetActive(false);
        }
        else if(hit.collider != null)
        {
            ascendIndicator.SetActive(true);
        }
    }

    public void MaterialColorChange()
    {
        if(hit.point.y - transform.position.y > 15)
        {
            indicatorMaterial.DOColor(Color.red, 0.1f);
        }
        else
        {  
            indicatorMaterial.DOColor(Color.green, 0.1f);
        }
    }

    //stopping ascend and ascendVFX
    void OnTriggerExit(Collider other) 
    {
        if(other.CompareTag("Wall"))
        {
            stopAscend = true;

            //vfx needs to be disabled when ascend is stopped (maybe its wrong place to call this method)
            ascendVFX.DisableRendererFeatures();
            ascendParticle.SetActive(false);
            ascendSpiralParticle.SetActive(false);
            ascendSpiralParticleFlipped.SetActive(false);

            Anim.SetBool("isAscending", false);
        }   
    }

    //reset back all the ascend bool variables
    public void climbAscendToGround()
    {
        anim.SetBool("Climb", false);
        climbButtonPressed = false;
        isAscendUsed = false;
        isAscendPressed = false;
        switchGroundState = true;
        CharacterController.enabled = true;

    }
    
}
