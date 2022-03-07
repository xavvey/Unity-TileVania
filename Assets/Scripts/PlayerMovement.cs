using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] float horizontalSpeed = 7.5f;
    [SerializeField] float verticalSpeed = 5f;
    [SerializeField] float jumpSpeed = 12f;
    [SerializeField] float climbingHorizontalSpeed = 2f;
    [Header("Player Life")]
    [SerializeField] int playerHealth = 2;
    [SerializeField] float hitFlashTime = 0.3f;
    [SerializeField] Vector2 hitFling = new Vector2(5f, 10f);
    [Header("Player Weapon")]
    [SerializeField] GameObject projectile;
    [SerializeField] Transform projectileSpawner;
    [SerializeField] float shootDelay = 0.5f;

    Vector2 moveInput;
    Rigidbody2D rbPlayer;
    Animator playerAnimator;
    CapsuleCollider2D playerCapsuleCollider;
    BoxCollider2D playerFeetBoxCollider;
    SpriteRenderer playerSpriteRenderer;
    Color defaultPlayerSpriteColor;

    bool playerHasHorizontalSpeed = false;
    bool playerHasVerticalSpeed = false;
    bool isAttachedToClimbing = false;
    bool isAlive = true;
    bool canShoot = true;
    int currentHealth;
    float defaultPlayerGravity;
    float defaultHorizontalSpeed;
    float defaultClimbAnimMultiplier;
    

    void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        rbPlayer = GetComponent<Rigidbody2D>();
        playerCapsuleCollider = GetComponent<CapsuleCollider2D>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        playerFeetBoxCollider = GetComponentInChildren<BoxCollider2D>();
    }

    void Start() 
    {
        defaultPlayerGravity = rbPlayer.gravityScale; 
        defaultHorizontalSpeed = horizontalSpeed;
        currentHealth = playerHealth;
        defaultClimbAnimMultiplier = playerAnimator.GetFloat("climbingAnimSpeed");
        defaultPlayerSpriteColor = playerSpriteRenderer.color;
    }
    
    void Update()
    {
        if (!isAlive) { return; } 

        Run();
        Climb();
        ProcessHit();
    }

    void FixedUpdate() 
    {
        
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

    void OnFire(InputValue value)
    {
        if (!isAlive) { return; } 
        if (!canShoot) { return; } 

        if (value.isPressed)
        {
            SpriteRenderer projectileSpriteRenderer = projectile.GetComponent<SpriteRenderer>();
            projectileSpriteRenderer.flipX = rbPlayer.transform.localScale.x < Mathf.Epsilon ? true : false;

            Instantiate(projectile, projectileSpawner.position, transform.rotation);
            playerAnimator.SetTrigger("Shoot");
            canShoot = false;
            StartCoroutine(ShootDelay());
        }
        else 
        {
            playerAnimator.ResetTrigger("Shoot");
        }   
    }

    IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(shootDelay);
        canShoot = true;
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
        if (!isAlive) { return; } 

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

    void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Enemy")
        {
           rbPlayer.velocity = hitFling;
            --currentHealth;

            if (isAlive && currentHealth > 0)
            {
                playerSpriteRenderer.color = Color.red;
                StartCoroutine(RestorePlayerSpriteColor());
            }
            else
            {
                Die();
            }
        }  
    }

    void ProcessHit()
    {
        if (rbPlayer.IsTouchingLayers(LayerMask.GetMask("Hazards")))
        {
            rbPlayer.velocity = hitFling;
            Die();
        }
        
        if (rbPlayer.IsTouchingLayers(LayerMask.GetMask("Water")))
        {
            Die();
        }
    }

    void Die() 
    {
        isAlive = false;
        playerAnimator.SetTrigger("Dying");
        playerSpriteRenderer.color = Color.red;
    }

    IEnumerator RestorePlayerSpriteColor()
    {
       yield return new WaitForSeconds(hitFlashTime);
       playerSpriteRenderer.color = defaultPlayerSpriteColor;
    }
}
