using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovement : MonoBehaviour
{
    // declare relerence variables
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    //Hash Optmize
    int isWalkingHash;
    int isRunningHash;

    // variables to store player input values
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    bool isMovementPressed;
    bool isRunPressed;
    float rotationFactorPerframe = 10.0f;
    float runMultiplier = 3.1f;
  

    //Awaked is called earlier than start in the event life cycle
    void Awake()
    {
        // initially set reference variables
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        //asign Hash fuctions in awake
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");

        // callbacks to playerinput
        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Run.started += onRun;
        playerInput.CharacterControls.Run.canceled += onRun;
   
    }

    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    //Turn Character
    void handleRotation()
    {
        Vector3 positionToLookAt;
        //change is position our character should point to
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;
        // the current roation of character
        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            //creates a new rotation based on where the player is currently pressing
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerframe * Time.deltaTime);
        }

    }


    // handler fuction to set the player input values
    void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        currentRunMovement.x = currentMovementInput.x * runMultiplier;
        currentRunMovement.z = currentMovementInput.y * runMultiplier;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;

    }

    void handleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        // start walking if movement pressed is true and not already true
        if (isMovementPressed && !isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        }

        else if (!isMovementPressed && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
        }
        // If movement is made and run is pressed
        if ((isMovementPressed && isRunPressed) && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }

        //stop runninf if movement or run are not pressed
        else if ((!isMovementPressed || !isRunPressed) && isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }

    }

    //Gravity
    void handleGravity()
    {
        if (characterController.isGrounded)
        {
            float groundedGravity = -.05f;
            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        }
        else
        {
            float gravity = -9.8f;
            currentMovement.y += gravity;
            currentRunMovement.y += gravity;
        }
    }


    // Update is called once per frame
    public void Update()
    {
        handleRotation();
        handleAnimation();
        handleGravity();

        if (isRunPressed)
        {
            characterController.Move(currentRunMovement * Time.deltaTime);
        }
        else
        {
            characterController.Move(currentMovement * Time.deltaTime);
        }

    }

    void OnEnable()
    {
        // enable the character controls action map
        playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        //disable the character controls action map
        playerInput.CharacterControls.Disable();
    }
}
