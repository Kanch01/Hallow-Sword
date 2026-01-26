using UnityEngine;
using TMPro;

public class BossStateTimerUI : MonoBehaviour
{
    public BossController boss;
    public TextMeshProUGUI timerText;
    
    private string invincibleLabel = "Invincible";
    private string vulnerableLabel = "Vulnerable";
    private Color invincibleColor = Color.indianRed;
    private Color vulnerableColor = Color.lightGreen;

    private void Update()
    {
        if (boss == null || timerText == null)
            return;
        
        float t = Mathf.Max(0f, boss.CurrentStateTimeRemaining);
        string stateLabel = boss.IsCurrentlyInvincible ? invincibleLabel : vulnerableLabel;
        
        timerText.text = $"{stateLabel}: {t:F1}s";
        timerText.color = boss.IsCurrentlyInvincible ? invincibleColor : vulnerableColor;
    }
}

