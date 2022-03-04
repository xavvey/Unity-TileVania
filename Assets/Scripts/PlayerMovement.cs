using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player tuning")]
    [SerializeField] float horizontalSpeed = 7.5f;
    [SerializeField] float verticalSpeed = 5f;
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float climbingHorizontalSpeed = 2f;

    Vector2 moveInput;
    Rigidbody2D rbPlayer;
    Animator playerAnimator;
    CapsuleCollider2D playerCapsuleCollider;

    bool playerHasHorizontalSpeed = false;
    bool playerHasVerticalSpeed = false;
    bool isAttachedToClimbing = false;
    float defaultPlayerGravity;
    float defaultHorizontalSpeed;
    float defaultClimbAnimMultiplier;

    void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        rbPlayer = GetComponent<Rigidbody2D>();
        playerCapsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    void Start() 
    {
        defaultPlayerGravity = rbPlayer.gravityScale; 
        defaultHorizontalSpeed = horizontalSpeed;
        defaultClimbAnimMultiplier = playerAnimator.GetFloat("climbingAnimSpeed");
    }
    
    void Update()
    {
        Run();
        Climb();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!playerCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) return;

        if(value.isPressed)
        {
            rbPlayer.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * horizontalSpeed, rbPlayer.velocity.y);
        rbPlayer.velocity = playerVelocity;

        FlipSprite();
        playerAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void FlipSprite()
    {        
        playerHasHorizontalSpeed = Mathf.Abs(rbPlayer.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed) 
        {
            transform.localScale = new Vector2(Mathf.Sign(rbPlayer.velocity.x), 1f);
        }
    }

    void Climb()
    {        
        if (!playerCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            ResetClimbingState(); 
            return;
        }
        else if (playerCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")) && moveInput.y > Mathf.Epsilon)
        {
            isAttachedToClimbing = true;
        }

        if (isAttachedToClimbing)
        {
            Vector2 climbVelocity = new Vector2(rbPlayer.velocity.x , moveInput.y * verticalSpeed);
            rbPlayer.velocity = climbVelocity;    
            rbPlayer.gravityScale = 0f;
            horizontalSpeed = climbingHorizontalSpeed;

            playerHasVerticalSpeed = Mathf.Abs(rbPlayer.velocity.y) > Mathf.Epsilon;
            if(playerHasVerticalSpeed)
            {
                playerAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
                playerAnimator.SetFloat("climbingAnimSpeed", defaultClimbAnimMultiplier);
            }
            else if (!playerHasVerticalSpeed)
            {
                playerAnimator.SetFloat("climbingAnimSpeed", 0f);
            }
        }

    }

    void ResetClimbingState()
    {
        isAttachedToClimbing = false;
        horizontalSpeed = defaultHorizontalSpeed;
        rbPlayer.gravityScale = defaultPlayerGravity;
        playerAnimator.SetBool("isClimbing", false);
        playerAnimator.SetFloat("climbingAnimSpeed", defaultClimbAnimMultiplier);
    }
}
