using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Slider healthSlider;
    public TMP_Text healthTMPText;

    private void Awake()
    {
        if (healthSlider == null)
            healthSlider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += HandleHealthChanged;
            playerHealth.OnDeath += HandlePlayerDeath;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= HandleHealthChanged;
            playerHealth.OnDeath -= HandlePlayerDeath;
        }
    }

    private void Start()
    {
        if (playerHealth != null)
        {
            healthSlider.maxValue = playerHealth.maxHealth;
            healthSlider.value = playerHealth.CurrentHealth;
            UpdateText(playerHealth.CurrentHealth);
        }
    }

    private void HandleHealthChanged(int current)
    {
        healthSlider.value = current;
        UpdateText(current);
    }

    private void HandlePlayerDeath()
    {
        healthSlider.value = 0;
        UpdateText(0);
    }

    private void UpdateText(int current)
    {
        string text = $"Player: {current} / {playerHealth.maxHealth}";

        if (healthTMPText != null)
            healthTMPText.text = text;
    }
}

