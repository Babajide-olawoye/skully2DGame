using System;
using JetBrains.Annotations;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

// Player movement controller script
public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb; // Reference to the player's Rigidbody2D component
    bool isFacingRight = true; // Tracks the direction the player is facing
    public Animator animator; // Reference to player's Animator component
    [Header("Moving")]
    public float moveSpeed = 5f; // Player's horizontal move speed
    float horizontalMovement;    // Stores the player's input (left/right)

    [Header("Jumping")]
    public float jumpPower = 10f; // Vertical force applied when jumping
    public int maxJump = 2;       // Max number of jumps (for double jump)
    int jumpRemaining;           // Tracks how many jumps are left

    [Header("Ground check")]
    public Transform groundCheckPas;                // Position for ground check box
    public Vector2 groundCheckSize = new(0.5f, 0.05f); // Size of the ground check box
    public LayerMask groundLayer;                   // Layer mask for ground detection
    bool isGrounded;                                // Is the player touching the ground?

    [Header("Wall Check")]
    public LayerMask wallLayer;                     // Layer mask for wall detection
    public Transform wallCheckPas;                  // Position for wall check box
    public Vector2 wallCheckSize = new(0.5f, 0.05f); // Size of the wall check box

    [Header("Gravity")]
    public float basedGravity = 2f;                 // Default gravity scale
    public float maxFallSpeed = -10f;               // Max speed player can fall
    public float fallSpeedMultiplier = 2f;          // Faster falling multiplier

    [Header("WallMovement")]
    public int wallSpeedSlider = 2;                 // Slowdown factor for wall slide
    bool isWalled;                                  // Is the player touching a wall?
    //Wall Jumping var
    bool isWallJumping;
    float wallJumpingDirection;
    float wallJumpTime = 0.5F;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new (5f, 10f);   //Jumping off wall power

    // Called once at the start of the game
    void Start()
    {
        // Initialization can go here if needed
    }

    // Called every frame
    void Update()
    {

        // Run checks and apply physics behaviors
        GroundCheck();
        WallCheck();
        Gravity();
        Flip();
        WallSpdSlider();
        ProcessWallJump();

        if (!isWallJumping)
        {
            // Apply horizontal movement
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
            Flip();
        }

        //Animators
        animator.SetFloat("magnitude", rb.linearVelocity.magnitude);
        animator.SetFloat("yVelocity", rb.linearVelocityY);
        animator.SetBool("wallSliding", isWalled);
    }

    // Called by the Input System when move input is received
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    // Called by the Input System when jump input is triggered
    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpRemaining > 0)
        {

            if (context.performed)
            {
                // Normal jump
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpPower);
                jumpRemaining--;
                animator.SetTrigger("jump");
            }
            else if (context.canceled)
            {
                // Short hop (cut jump)
                rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * 0.5f);
                jumpRemaining--;
                animator.SetTrigger("jump");

            }
        }

        //Makes us jump away from the wall
        if (context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpingDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpTimer = 0;
            animator.SetTrigger("jump");

            //Force flip
            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1f;
                transform.localScale = ls;
            }

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
        }
    }

    private void ProcessWallJump()
    {
        if (isWalled)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    // Adjust gravity based on whether the player is falling
    public void Gravity()
    {
        if (rb.linearVelocityY < 0)
        {
            // Faster falling and clamp to max fall speed
            rb.gravityScale = basedGravity * fallSpeedMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, MathF.Max(rb.linearVelocityY, maxFallSpeed));
        }
        else
        {
            // Normal gravity when going up or idle
            rb.gravityScale = basedGravity;
        }
    }

    // Visual debugging: draws ground and wall check boxes around player
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPas.position, groundCheckSize);
        Gizmos.DrawWireCube(wallCheckPas.position, wallCheckSize);
    }

    // Checks if the player is touching the ground
    public void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPas.position, groundCheckSize, 0, groundLayer))
        {
            isGrounded = true;
            jumpRemaining = maxJump; // Reset jumps when grounded
        }
        else
        {
            isGrounded = false;
        }
    }

    // Checks if the player is touching a wall
    public void WallCheck()
    {
        if (Physics2D.OverlapBox(wallCheckPas.position, wallCheckSize, 0, wallLayer))
        {
            isWalled = true;
            jumpRemaining = maxJump; // reset jumps when on wall
            Debug.Log(true);
        }
        else
        {
            isWalled = false;
        }
    }

    // Flips the player object based on movement direction
    public void Flip()
    {
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    // Slows the fall speed when sliding down a wall
    public void WallSpdSlider()
    {
        if (isWalled == true & !isGrounded & horizontalMovement != 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY / 2);
            // Debug.Log("Wall slide");
        }
    }
}
