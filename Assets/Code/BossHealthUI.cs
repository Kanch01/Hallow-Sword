using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class BossHealthUI : MonoBehaviour
{
    public BossHealth bossHealth;
    public Slider healthSlider;
    public TMP_Text healthTMPText;

    private void Awake()
    {
        if (healthSlider == null)
            healthSlider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged += HandleHealthChanged;
            bossHealth.OnDeath += HandleBossDeath;
        }
    }

    private void OnDisable()
    {
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged -= HandleHealthChanged;
            bossHealth.OnDeath -= HandleBossDeath;
        }
    }

    private void Start()
    {
        if (bossHealth != null)
        {
            healthSlider.maxValue = bossHealth.maxHealth;
            healthSlider.value = bossHealth.CurrentHealth;
            UpdateText(bossHealth.CurrentHealth);
        }
    }

    private void HandleHealthChanged(int current)
    {
        healthSlider.value = current;
        UpdateText(current);
    }

    private void HandleBossDeath()
    {
        healthSlider.value = 0;
        UpdateText(0);
        
        gameObject.SetActive(false);
    }

    private void UpdateText(int current)
    {
        string text = $"Boss: {current} / {bossHealth.maxHealth}";
        
        if (healthTMPText != null)
            healthTMPText.text = text;
    }
}

