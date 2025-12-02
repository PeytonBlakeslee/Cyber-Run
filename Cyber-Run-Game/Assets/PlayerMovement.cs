using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform feetPos;
    [SerializeField] private Transform Robot;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;   // lower this to reduce jump height
    [SerializeField] private float jumpTime  = 0.25f; // lower this to reduce how long you "boost" while holding jump

    [Header("Crouch Settings")]
    [SerializeField] private float crouchHeight = 3.5f;
    [SerializeField] private float standHeight  = 5f;
    [SerializeField] private float crouchSpeed  = 15f; // higher = faster transition

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundDistance = 0.25f;

    // state
    private bool isGrounded;
    private bool wasGrounded;
    private bool isJumping;
    private float jumpTimer;
    private bool isCrouching;
    private bool crouchRequested;

    // Hard-mode-only helper: track whether crouch was held last frame
    // so we can require a "fresh press" to re-initiate crouch
    private bool crouchHeldLastFrame;

    private void Start()
    {
        // When a new run starts, fully reset movement / crouch state
        GameManager.Instance.onPlay.AddListener(ResetMovementState);
    }

    // Clears all transient state so a new run always starts standing
    private void ResetMovementState()
    {
        isGrounded = false;
        wasGrounded = false;
        isJumping = false;
        jumpTimer = 0f;
        isCrouching = false;
        crouchRequested = false;
        crouchHeldLastFrame = false;

        // snap visual scale back to standing height
        if (Robot != null)
            Robot.localScale = new Vector3(Robot.localScale.x, standHeight, Robot.localScale.z);
    }

    private void Update()
    {
        // update ground state every frame
        isGrounded = Physics2D.OverlapCircle(feetPos.position, groundDistance, groundLayer);

        // block movement / input when game is not playing
        if (!GameManager.Instance.isPlaying)
        {
            wasGrounded = isGrounded;
            isJumping = false;
            jumpTimer = 0f;
            crouchRequested = false;
            isCrouching = false;
            crouchHeldLastFrame = false;

            // optional: keep the robot visually at standing height while not playing
            if (Robot != null)
                Robot.localScale = new Vector3(Robot.localScale.x, standHeight, Robot.localScale.z);

            return;
        }

        // which difficulty are we in right now?
        bool isHard =
            GameManager.Instance.difficulty == GameManager.DifficultyMode.Hard;

        // --- input cache ---
        bool jumpDown   = Input.GetButtonDown("Jump");
        bool jumpHeld   = Input.GetButton("Jump");
        bool jumpUp     = Input.GetButtonUp("Jump");

        bool crouchHeld = Input.GetButton("Crouch");
        bool crouchUp   = Input.GetButtonUp("Crouch");

        #region JUMPING ---
        // In Normal: you can jump from crouch (original behavior).
        // In Hard: you cannot jump while crouching or holding crouch.
        bool canStartJump = isGrounded && jumpDown;

        if (isHard)
        {
            // Disallow jump if crouching or crouch is currently held
            if (isCrouching || crouchHeld)
                canStartJump = false;
        }

        // start jump (jump cancels crouch and its request when allowed)
        if (canStartJump)
        {
            isCrouching = false;
            crouchRequested = false;
            isJumping = true;

            rb.linearVelocity = Vector2.up * jumpForce;
            jumpTimer = 0f;

            // Play jump SFX
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayJump();
        }

        // sustain variable jump height
        if (isJumping && jumpHeld)
        {
            if (jumpTimer < jumpTime)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpTimer += Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        // stop jump
        if (jumpUp)
        {
            isJumping = false;
            jumpTimer = 0f;
        }
        #endregion

        #region CROUCHING ---
        if (!isHard)
        {
            // NORMAL MODE: original behavior (hold = stay crouched)
            // buffer request while held, clear on release
            if (crouchHeld) crouchRequested = true;
            if (crouchUp)   crouchRequested = false;
        }
        else
        {
            // HARD MODE:
            // - Require a fresh press to initiate crouch
            // - Holding crouch across a jump won't auto-crouch again on landing

            bool crouchDownThisFrame = crouchHeld && !crouchHeldLastFrame;

            if (crouchDownThisFrame)
                crouchRequested = true;

            if (crouchUp)
                crouchRequested = false;
        }

        // auto-crouch on landing if still requested
        if (isGrounded && !wasGrounded && crouchRequested)
            isCrouching = true;

        // while grounded, apply request unless we're in the jump-start frame
        if (isGrounded && !isJumping)
            isCrouching = crouchRequested;

        // remember ground state
        wasGrounded = isGrounded;

        // visuals (smooth scale toward target)
        float targetY  = isCrouching ? crouchHeight : standHeight;
        float currentY = Robot.localScale.y;
        float newY     = Mathf.Lerp(currentY, targetY, Time.deltaTime * crouchSpeed);
        Robot.localScale = new Vector3(Robot.localScale.x, newY, Robot.localScale.z);

        // store crouchHeld for next frame (used by Hard mode logic)
        crouchHeldLastFrame = crouchHeld;
        #endregion
    }
}
