using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player tuning")]
    [SerializeField] float horizontalSpeed = 7.5f;
    [SerializeField] float verticalSpeed = 5f;
    [SerializeField] float jumpSpeed = 12f;
    [SerializeField] float climbingHorizontalSpeed = 2f;
    [SerializeField] int playerHealth = 2;

    Vector2 moveInput;
    Rigidbody2D rbPlayer;
    Animator playerAnimator;
    CapsuleCollider2D playerCapsuleCollider;
    BoxCollider2D playerFeetBoxCollider;
    SpriteRenderer playerSpriteRenderer;
    Color defaultPlayerSpriteColor = new Color(255f, 255f, 255f, 255f);

    bool playerHasHorizontalSpeed = false;
    bool playerHasVerticalSpeed = false;
    bool isAttachedToClimbing = false;
    bool isAlive = true;
    int currentHealth;
    float defaultPlayerGravity;
    float defaultHorizontalSpeed;
    float defaultClimbAnimMultiplier;
    

    void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        rbPlayer = GetComponent<Rigidbody2D>();
        playerCapsuleCollider = GetComponent<CapsuleCollider2D>();
        playerFeetBoxCollider = GetComponent<BoxCollider2D>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start() 
    {
        defaultPlayerGravity = rbPlayer.gravityScale; 
        defaultHorizontalSpeed = horizontalSpeed;
        currentHealth = playerHealth;
        defaultClimbAnimMultiplier = playerAnimator.GetFloat("climbingAnimSpeed");
    }
    
    void Update()
    {
        if (!isAlive) { return; } 

        Run();
        Climb();
        OnHit();
    }

    void OnMove(InputValue value)
    {
        if (!isAlive) { return; } 
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!playerFeetBoxCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) || !isAlive) return;

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
        if (!playerFeetBoxCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            ResetClimbingState(); 
            return;
        }
        else if (playerFeetBoxCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")) && moveInput.y > Mathf.Epsilon)
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

    void OnHit()
    {        
        Vector2 hitVelocity = new Vector2(-(transform.localScale.x) * 20f, 10f);
        if (playerCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Enemies")) && currentHealth > 0)
        {
            rbPlayer.velocity = hitVelocity;           
            playerSpriteRenderer.material.color = Color.red;
            Invoke("SubtractPlayerHealth", 0.1f);
        }
        else if (currentHealth == 0)
        {
            isAlive = false;
            playerAnimator.SetTrigger("Dying");
        }
        
        Invoke("RestorePlayerSpriteColor", 0.1f); // TODO: Invoke not working for restoring sprite color!
    }

    void SubtractPlayerHealth()
    {
        currentHealth--;
    }

    void RestorePlayerSpriteColor()
    {
        playerSpriteRenderer.color = new Color(255f, 255f, 255f, 255f);
        Debug.Log(defaultPlayerSpriteColor);
    }
}
