using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    public Transform player;

    private Rigidbody2D rb;
    private BossHealth health;
    private Animator animator;
    
    public float phase2Speed = 4f;
    public float phase1Speed = 7f;
    public float phase0Speed = 13f;
    
    public float dashSpeed = 20f;
    public float dashWindupTime = 0.3f;
    public float dashDuration = 0.5f;
    
    public float phase2InvincibleTime = 8f;
    public float phase2VulnerableTime = 4f;

    public float phase1InvincibleTime = 15f;
    public float phase1VulnerableTime = 3f;

    public float phase0InvincibleTime = 20f;
    public float phase0VulnerableTime = 1.5f;
    
    public float cooldownBetweenPatterns = 1.0f;

    private bool isPerformingPattern;
    private bool invincible;
    
    public int contactDamage = 1;
    public LayerMask playerLayer;
    
    public Camera targetCamera;
    public float screenPadding = 0.1f;
    private CapsuleCollider2D bossCollider;
    public bool IsCurrentlyInvincible => invincible;
    public float CurrentStateTimeRemaining { get; private set; }
    public float CurrentStateDuration { get; private set; }
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<BossHealth>();
        animator = GetComponent<Animator>();
        
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void OnEnable()
    {
        health.OnStageChanged += HandleStageChanged;
        health.OnInvincibilityChanged += HandleInvincibilityChanged;
    }

    private void OnDisable()
    {
        health.OnStageChanged -= HandleStageChanged;
        health.OnInvincibilityChanged -= HandleInvincibilityChanged;
    }

    private void Start()
    {
        HandleStageChanged(health.CurrentStage);
        HandleInvincibilityChanged(health.IsInvincible);
        
        StartCoroutine(InvincibilityCycle());
        StartCoroutine(BehaviorLoop());
    }

    private void HandleStageChanged(int stage)
    {

    }

    private void HandleInvincibilityChanged(bool invincible)
    {
        if (animator != null)
        {
            animator.SetBool("Invincible", invincible);
        }
    }

    private void FixedUpdate()
    {
        if (!isPerformingPattern && player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            float speed = GetCurrentPhaseSpeed();
            rb.linearVelocity = direction * speed;
        }
    }

    private float GetCurrentPhaseSpeed()
    {
        switch (health.CurrentStage)
        {
            case 2: return phase2Speed;
            case 1: return phase1Speed;
            case 0: return phase0Speed;
        }
        return phase2Speed;
    }

    private IEnumerator InvincibilityCycle()
    {
        while (health.CurrentHealth > 0)
        {
            GetPhaseTimes(out float invincibleTime, out float vulnerableTime);
            
            health.SetInvincible(true);
            invincible = true;

            CurrentStateDuration = invincibleTime;
            CurrentStateTimeRemaining = invincibleTime;

            while (CurrentStateTimeRemaining > 0f && health.CurrentHealth > 0)
            {
                CurrentStateTimeRemaining -= Time.deltaTime;
                yield return null;
            }
            
            CurrentStateTimeRemaining = 0f;
            
            health.SetInvincible(false);
            invincible = false;

            CurrentStateDuration = vulnerableTime;
            CurrentStateTimeRemaining = vulnerableTime;

            while (CurrentStateTimeRemaining > 0f && health.CurrentHealth > 0)
            {
                CurrentStateTimeRemaining -= Time.deltaTime;
                yield return null;
            }

            CurrentStateTimeRemaining = 0f;
        }
        
        CurrentStateTimeRemaining = 0f;
    }


    private void GetPhaseTimes(out float invincibleTime, out float vulnerableTime)
    {
        switch (health.CurrentStage)
        {
            case 2:
                invincibleTime = phase2InvincibleTime;
                vulnerableTime = phase2VulnerableTime;
                break;
            case 1:
                invincibleTime = phase1InvincibleTime;
                vulnerableTime = phase1VulnerableTime;
                break;
            case 0:
            default:
                invincibleTime = phase0InvincibleTime;
                vulnerableTime = phase0VulnerableTime;
                break;
        }
    }

        private IEnumerator BehaviorLoop()
    {
        while (health.CurrentHealth > 0)
        {
            switch (health.CurrentStage)
            {
                case 2:
                    yield return StartCoroutine(Pattern_SlowHomingRams());
                    break;
                case 1:
                    yield return StartCoroutine(Pattern_ZigZagRams());
                    break;
                case 0:
                    yield return StartCoroutine(Pattern_BarrageRams());
                    break;
            }

            yield return new WaitForSeconds(cooldownBetweenPatterns);
        }
    }
        
    private void ClampToCameraBounds()
    {
        if (targetCamera == null || bossCollider == null)
            return;
        
        Vector3 camPos = targetCamera.transform.position;
        float halfHeight = targetCamera.orthographicSize;
        float halfWidth = halfHeight * targetCamera.aspect;
        
        Bounds bounds = bossCollider.bounds;
        Vector3 pos = transform.position;

        float leftLimit = camPos.x - halfWidth + bounds.extents.x + screenPadding;
        float rightLimit = camPos.x + halfWidth - bounds.extents.x - screenPadding;
        float bottomLimit = camPos.y - halfHeight + bounds.extents.y + screenPadding;
        float topLimit = camPos.y + halfHeight - bounds.extents.y - screenPadding;

        pos.x = Mathf.Clamp(pos.x, leftLimit, rightLimit);
        pos.y = Mathf.Clamp(pos.y, bottomLimit, topLimit);

        transform.position = pos;
    }
    
    private IEnumerator Pattern_SlowHomingRams()
    {
        isPerformingPattern = true;
        rb.linearVelocity = Vector2.zero;

        int rams = 2;
        for (int i = 0; i < rams; i++)
        {
            if (player == null) break;

            Vector2 dir = (player.position - transform.position).normalized;
            
            yield return new WaitForSeconds(dashWindupTime);
            
            float timer = 0f;
            while (timer < dashDuration)
            {
                timer += Time.deltaTime;
                rb.linearVelocity = dir * dashSpeed;
                yield return new WaitForFixedUpdate();
            }

            rb.linearVelocity = Vector2.zero;
            yield return new WaitForSeconds(0.4f);
        }

        isPerformingPattern = false;
    }
    
    private IEnumerator Pattern_ZigZagRams()
    {
        isPerformingPattern = true;

        int waves = 3;
        float zigzagAmplitude = 2.5f;
        float zigzagDuration = 1.2f;

        for (int w = 0; w < waves; w++)
        {
            if (player == null) break;

            Vector2 toPlayer = (player.position - transform.position).normalized;
            Vector2 perpendicular = new Vector2(-toPlayer.y, toPlayer.x);

            float timer = 0f;
            while (timer < zigzagDuration)
            {
                timer += Time.deltaTime;
                float t = timer / zigzagDuration;

                Vector2 forward = toPlayer * dashSpeed;
                float sine = Mathf.Sin(t * Mathf.PI * 4f);
                Vector2 side = perpendicular * sine * zigzagAmplitude;

                rb.linearVelocity = forward + side;
                
                yield return new WaitForFixedUpdate();
            }

            rb.linearVelocity = Vector2.zero;
            yield return new WaitForSeconds(0.3f);
        }

        isPerformingPattern = false;
    }
    
    private IEnumerator Pattern_BarrageRams()
    {
        isPerformingPattern = true;

        int rams = 5;
        for (int i = 0; i < rams; i++)
        {
            if (player == null) break;

            Vector2 targetPos = player.position;
            Vector2 dir = (targetPos - (Vector2)transform.position).normalized;

            float localWindup = dashWindupTime * 0.6f;
            float localDuration = dashDuration * 0.8f;
            
            yield return new WaitForSeconds(localWindup);
            
            float timer = 0f;
            while (timer < localDuration)
            {
                timer += Time.deltaTime;
                rb.linearVelocity = dir * (dashSpeed * 1.5f);
                yield return new WaitForFixedUpdate();
            }

            rb.linearVelocity = Vector2.zero;
            yield return new WaitForSeconds(0.2f);
        }

        isPerformingPattern = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) == 0)
            return;

        var health = other.GetComponent<PlayerHealth>();
        if (health != null && invincible)
        {
            health.TakeDamage(contactDamage);
        }
    }
    
    private void LateUpdate()
    {
        ClampToCameraBounds();
    }

}

