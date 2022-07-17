using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;
    public Transform groundCheck;
    public Transform bonkCheck;

    public enum JumpStates { Standing, Jumping, Falling }
    [Header("Jumping")]
    public JumpStates jumpStatesToggle = JumpStates.Standing;

    public float jumpHeight = 3f;
    public float gravity = -9.81f;

    public enum RunStates { Walking, Gaining, Running }
    [Header("Running")]
    public RunStates runStatesToggle = RunStates.Walking;
    public float playerMovementSpeed;
    public float walkSpeed = 0f;
    public float runSpeed = 0f;

    public enum CrouchStates { Standing, Crouched, Crouching, Rising }
    [Header("Crouching")]
    public CrouchStates crouchStatesToggle = CrouchStates.Standing;
    public float standHeight = 3.8f;
    public float crouchHeight = 0.2f;
    public float crouchSpeed;
    public float cameraOffset;

    float velocityY;
    float jumpHeightModifier;
    float playerSpeedModifier;
    float startTime = 0;
    bool progressBool;
    public bool interactionCheck;

    Camera camera;
    [SerializeField] GroundChecker m_groundChecker = null;

    void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        Movement();

        CrouchToggle();
        RunToggle();
        PlayerStatAdjust();
        if (progressBool == true)
        {
            InteractionTimeProgress();
        }

        PositionAnchor(groundCheck, 2, false);
        PositionAnchor(camera.transform, 5.5f, true);
        PositionAnchor(bonkCheck, 2, true);
    }

    void PositionAnchor(Transform otherTransform, float offset, bool positive)
    {
        if (positive == true)
            if ((characterController.height / offset) > characterController.radius)
                otherTransform.position = new Vector3(transform.position.x, transform.position.y + (characterController.height / offset), transform.position.z);
            else
                otherTransform.position = new Vector3(transform.position.x, transform.position.y + characterController.radius, transform.position.z);

        else
            if ((characterController.height / offset) > characterController.radius)
                otherTransform.position = new Vector3(transform.position.x, transform.position.y - (characterController.height / offset), transform.position.z);
            else
                otherTransform.position = new Vector3(transform.position.x, transform.position.y - characterController.radius, transform.position.z);
    }

    void Movement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        switch (jumpStatesToggle)
        {
            case JumpStates.Standing:
                if (velocityY < 0)
                    velocityY = -2f;
                if (Input.GetButtonDown("Jump"))
                {
                    velocityY = Mathf.Sqrt((jumpHeight - jumpHeightModifier) * -2f * gravity);
                    jumpStatesToggle = JumpStates.Jumping;
                }
                break;
        }

        velocityY += gravity * Time.deltaTime;
        Vector3 move = (transform.right * x * playerMovementSpeed) + (transform.forward * z * playerMovementSpeed) + (transform.up * velocityY);
        Vector3.ClampMagnitude(move, playerMovementSpeed);
        characterController.Move(move * Time.deltaTime);

    }
    void InteractionTimeProgress()
    {
        Debug.Log(startTime - Time.time);
    }

    void CrouchToggle()
    {
        switch (crouchStatesToggle)
        {
            case CrouchStates.Standing:
                characterController.height = standHeight;
                if (Input.GetKeyDown(KeyCode.LeftControl))
                    crouchStatesToggle = CrouchStates.Crouching;
                break;

            case CrouchStates.Crouched:
                characterController.height = crouchHeight;
                if (Input.GetKeyDown(KeyCode.LeftControl))
                    crouchStatesToggle = CrouchStates.Rising;
                break;

            case CrouchStates.Crouching:
                characterController.height = Mathf.Lerp(characterController.height, crouchHeight, crouchSpeed);
                if (Mathf.Abs(characterController.height - crouchHeight) <= 0.2f)
                    crouchStatesToggle = CrouchStates.Crouched;
                break;

            case CrouchStates.Rising:
                characterController.height = Mathf.Lerp(characterController.height, standHeight, crouchSpeed);
                if (Mathf.Abs(characterController.height - standHeight) <= 0.2f)
                crouchStatesToggle = CrouchStates.Standing;
                break;
        }
    }

    void RunToggle()
    {
        switch (runStatesToggle)
        {
            case RunStates.Walking:
                if (Input.GetKey(KeyCode.LeftShift))
                    runStatesToggle = RunStates.Gaining;
                playerMovementSpeed = walkSpeed - playerSpeedModifier;
                break;
            case RunStates.Gaining:
                if (Input.GetKeyUp(KeyCode.LeftShift))
                    runStatesToggle = RunStates.Walking;
                playerMovementSpeed = Mathf.Lerp(playerMovementSpeed, runSpeed - playerSpeedModifier, 0.1f);
                if (playerMovementSpeed == runSpeed - playerSpeedModifier)
                    runStatesToggle = RunStates.Running;
                break;
            case RunStates.Running:
                if (Input.GetKeyUp(KeyCode.LeftShift))
                    runStatesToggle = RunStates.Walking;
                playerMovementSpeed = runSpeed - playerSpeedModifier;
                break;
        }
    }

    void PlayerStatAdjust(float playerSpeedModifierValue = 0, float jumpHeightModifierValue = 0)
    {
        jumpHeightModifier += jumpHeightModifierValue;
        playerSpeedModifier += playerSpeedModifierValue;
    }

    public void TriggerFalling()
    {
        jumpStatesToggle = JumpStates.Falling;
    }

    public void TriggerLanding()
    {
        jumpStatesToggle = JumpStates.Standing;
    }

    // Stub function for AI. Not currently used but it is called.
    public void PushPlayer(Vector3 pushStrength, float pushSmoothTime)
    {

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(m_groundChecker.CheckGround())
        {
            m_groundChecker.EnterRadius();
        }
    }
}