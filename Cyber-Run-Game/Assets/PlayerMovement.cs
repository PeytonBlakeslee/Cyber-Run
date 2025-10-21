using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform feetPos;
    [SerializeField] private Transform Robot;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpTime = 0.25f;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchHeight = 3.5f;
    [SerializeField] private float standHeight = 5f;
    [SerializeField] private float crouchSpeed = 15f; // higher = faster transition

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

    private void Update()
    {
        // --- input cache ---
        bool jumpDown = Input.GetButtonDown("Jump");
        bool jumpHeld = Input.GetButton("Jump");
        bool jumpUp = Input.GetButtonUp("Jump");

        bool crouchHeld = Input.GetButton("Crouch");
        bool crouchUp = Input.GetButtonUp("Crouch");

        // --- ground check ---
        isGrounded = Physics2D.OverlapCircle(feetPos.position, groundDistance, groundLayer);


        #region JUMPING ---
        // start jump (jump cancels crouch and its request)
        if (isGrounded && jumpDown)
        {
            isCrouching = false;
            crouchRequested = false;
            isJumping = true;

            rb.linearVelocity = Vector2.up * jumpForce;              // keep original zeroing of X
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
        // buffer request while held, clear on release
        if (crouchHeld) crouchRequested = true;
        if (crouchUp) crouchRequested = false;

        // auto-crouch on landing if still requested
        if (isGrounded && !wasGrounded && crouchRequested)
            isCrouching = true;

        // while grounded, apply request unless we’re in the jump-start frame
        if (isGrounded && !isJumping)
            isCrouching = crouchRequested;

        // remember ground state
        wasGrounded = isGrounded;

        // visuals (smooth scale toward target)
        float targetY = isCrouching ? crouchHeight : standHeight;
        float currentY = Robot.localScale.y;
        float newY = Mathf.Lerp(currentY, targetY, Time.deltaTime * crouchSpeed);
        Robot.localScale = new Vector3(Robot.localScale.x, newY, Robot.localScale.z);
    }
    #endregion
}