using UnityEngine;
using System;
using System.Collections;
using TMPro;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 300;
    public int CurrentHealth { get; private set; }
    public TextMeshProUGUI finalText;

    public bool IsInvincible { get; private set; }
    
    public int CurrentStage { get; private set; } = 2;

    public event Action<int> OnHealthChanged;
    public event Action<int> OnStageChanged;
    public event Action<bool> OnInvincibilityChanged;
    public event Action OnDeath;
    private SpriteRenderer _renderer;
    private Color originalColor;
    private Coroutine hurtroutine;
    public AudioClip hurtsfx;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        CurrentStage = GetStageFromHealth(CurrentHealth);
        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer != null)
        {
            originalColor = _renderer.color;
        }
    }

    public void TakeDamage(int amount)
    {
        if (IsInvincible || amount <= 0) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        
        OnHealthChanged?.Invoke(CurrentHealth);

        int newStage = GetStageFromHealth(CurrentHealth);
        if (newStage != CurrentStage)
        {
            CurrentStage = newStage;
            OnStageChanged?.Invoke(CurrentStage);
        }

        if (CurrentHealth == 0)
        {
            finalText.enabled = true;
            finalText.text = "YOU WIN";
            OnDeath?.Invoke();
            Destroy(gameObject);
        }

        hurtroutine = StartCoroutine(HurtFeedbackCoroutine());
    }

    private IEnumerator HurtFeedbackCoroutine()
    {
        _renderer.color = Color.red;
        AudioManager.Instance.PlaySFX(hurtsfx);
        yield return new WaitForSeconds(1);
        _renderer.color = originalColor;
        hurtroutine = null;
    }

    private int GetStageFromHealth(int hp)
    {
        float fraction = (float)hp / maxHealth;

        if (fraction > 2f / 3f) return 2;
        if (fraction > 1f / 3f) return 1;
        return 0;
    }

    public void SetInvincible(bool invincible)
    {
        if (IsInvincible == invincible) return;

        IsInvincible = invincible;
        OnInvincibilityChanged?.Invoke(invincible);
    }
}

