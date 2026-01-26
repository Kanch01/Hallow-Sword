using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 14f;
    
    public float jumpForce = 20f;
    public int maxJumps = 2;
    
    public float dashSpeedMultiplier = 6f;
    public float dashDuration = 0.04f;
    public float dashCooldown = 0.5f;
    
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    
    public bool canAttackInAir = true;
    
    public bool enableFastFall = true;
    public float fastFallMultiplier = 20f;
    public float fastFallInputThreshold = -0.7f;

    private Rigidbody2D rb;
    private Animator anim;
    private InputSystem_Actions controls;

    private Vector2 moveInput;
    private bool isGrounded;
    private int jumpCount;
    private bool facingRight = true;
    
    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    
    private bool wantsFastFall;
    
    public Collider2D attack1Hitbox;
    public Collider2D attack2Hitbox;

    public AudioClip dashsfx;
    public AudioClip hitsfx;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        controls = new InputSystem_Actions();
        
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;
        
        controls.Player.Jump.started += OnJump;
        
        controls.Player.Dash.started += OnDash;
        
        controls.Player.Attack1.started += OnAttack1;
        controls.Player.Attack2.started += OnAttack2;

        if (attack1Hitbox) attack1Hitbox.enabled = false;
        if (attack2Hitbox) attack2Hitbox.enabled = false;
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void OnDestroy()
    {
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMove;
        controls.Player.Jump.started -= OnJump;
        controls.Player.Dash.started -= OnDash;
        controls.Player.Attack1.started -= OnAttack1;
        controls.Player.Attack2.started -= OnAttack2;
    }

    private void Update()
    {
        CheckGround();
        UpdateDashTimers();
        
        if (moveInput.x > 0.01f && !facingRight)
        {
            Flip();
        }
        else if (moveInput.x < -0.01f && facingRight)
        {
            Flip();
        }

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        Vector2 velocity = rb.linearVelocity;

        if (isDashing)
        {
            float dashDir = facingRight ? 1f : -1f;
            velocity.x = dashDir * moveSpeed * dashSpeedMultiplier;
            AudioManager.Instance.PlaySFX(dashsfx);
        }
        else
        {
            velocity.x = moveInput.x * moveSpeed;
            
            if (enableFastFall && !isGrounded && velocity.y < 0f && wantsFastFall)
            {
                float extraGravity = Physics2D.gravity.y * (fastFallMultiplier - 1f);
                velocity.y += extraGravity * Time.fixedDeltaTime;
            }
        }

        rb.linearVelocity = velocity;
    }

    private void CheckGround()
    {
        bool wasGrounded = isGrounded;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (isGrounded && !wasGrounded)
        {
            jumpCount = 0;
        }
    }

    private void UpdateDashTimers()
    {
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
        }
    }
    
    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            moveInput = ctx.ReadValue<Vector2>();
        }
        else if (ctx.phase == InputActionPhase.Canceled)
        {
            moveInput = Vector2.zero;
        }
        
        wantsFastFall = moveInput.y <= fastFallInputThreshold;
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (isGrounded || jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            jumpCount++;

            anim.SetTrigger("Jump");
        }
    }

    private void OnDash(InputAction.CallbackContext ctx)
    {
        if (dashCooldownTimer > 0f) return;

        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
    }

    private void OnAttack1(InputAction.CallbackContext ctx)
    {
        TryAttack(1);
    }

    private void OnAttack2(InputAction.CallbackContext ctx)
    {
        TryAttack(2);
    }

    private void TryAttack(int attackNumber)
    {
        if (!isGrounded && !canAttackInAir)
            return;

        if (attackNumber == 1)
        {
            anim.SetTrigger("Attack1");
        }
        else
        {
            anim.SetTrigger("Attack2");
        }
        
        AudioManager.Instance.PlaySFX(hitsfx);
        
        if (isDashing)
        {
            isDashing = false;
        }
    }

    public void EnableAttack1Hitbox()
    {
        if (attack1Hitbox) attack1Hitbox.enabled = true;
    }

    public void DisableAttack1Hitbox()
    {
        if (attack1Hitbox) attack1Hitbox.enabled = false;
    }

    public void EnableAttack2Hitbox()
    {
        if (attack2Hitbox) attack2Hitbox.enabled = true;
    }

    public void DisableAttack2Hitbox()
    {
        if (attack2Hitbox) attack2Hitbox.enabled = false;
    }
    
    private void UpdateAnimator()
    {
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocityX));
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("VerticalVelocity", rb.linearVelocityY);
        anim.SetBool("IsDashing", isDashing);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}

