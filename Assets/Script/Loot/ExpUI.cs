using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ExpUI : MonoBehaviour
{
    public Image expFillBar;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI expText;

    void Start()
    {
        PlayerStats.OnExpChanged += UpdateExpUI;
        PlayerStats.OnLevelUp += HandleLevelUp;

        UpdateExpUI(PlayerStats.CurrentExp, PlayerStats.MaxExpForCurrentLevel);
        if (levelText != null) levelText.text = $"Lv. {PlayerStats.CurrentLevel}";
    }

    private void OnDestroy()
    {
        PlayerStats.OnExpChanged -= UpdateExpUI;
        PlayerStats.OnLevelUp -= HandleLevelUp;
    }

    private void UpdateExpUI(int currentExp, int requiredExp)
    {
        if (expFillBar != null) expFillBar.fillAmount = (float)currentExp / requiredExp;
        if (expText != null) expText.text = $"{currentExp} / {requiredExp}";
    }

    private void HandleLevelUp(int newLevel)
    {
        if (levelText != null) levelText.text = $"Lv. {newLevel}";
    }
}