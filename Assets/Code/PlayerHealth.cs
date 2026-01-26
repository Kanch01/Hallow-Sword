using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int CurrentHealth { get; private set; }
    
    public bool useInvincibilityFrames = true;
    public float invincibilityDuration = 1f;
    public Color hurtColor = Color.red;

    public event Action<int> OnHealthChanged;
    public event Action OnDeath;

    private bool isInvincible;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine invincibilityRoutine;
    public AudioClip hurtsfx;
    public TextMeshProUGUI finalText;

    private void Awake()
    {
        CurrentHealth = maxHealth;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        OnHealthChanged?.Invoke(CurrentHealth);
    }
    
    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        if (isInvincible && useInvincibilityFrames) return;
        if (CurrentHealth <= 0) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        OnHealthChanged?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (invincibilityRoutine != null)
                StopCoroutine(invincibilityRoutine);

            invincibilityRoutine = StartCoroutine(HurtFeedbackCoroutine());
        }
    }
    
    public void Heal(int amount)
    {
        if (amount <= 0 || CurrentHealth <= 0) return;

        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    private void Die()
    {
        finalText.enabled = true;
        OnDeath?.Invoke();
        AudioManager.Instance.StopMusic();
        
        Destroy(gameObject);
    }

    private IEnumerator HurtFeedbackCoroutine()
    {
        if (useInvincibilityFrames)
            isInvincible = true;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hurtColor;
            AudioManager.Instance.PlaySFX(hurtsfx);
        }
        
        yield return new WaitForSeconds(invincibilityDuration);
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        isInvincible = false;
        invincibilityRoutine = null;
    }
}


