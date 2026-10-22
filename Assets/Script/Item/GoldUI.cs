using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    public TextMeshProUGUI goldText;

    void Start()
    {
        // 订阅金币变化事件
        PlayerStats.OnGoldChanged += UpdateGoldUI;

        // 初始化显示当前金币
        UpdateGoldUI(PlayerStats.Gold);
    }

    private void OnDestroy()
    {
        // 取消订阅，防止内存泄漏
        PlayerStats.OnGoldChanged -= UpdateGoldUI;
    }

    private void UpdateGoldUI(int newGoldAmount)
    {
        if (goldText != null)
        {
            goldText.text = $"金币: {newGoldAmount}";
        }
    }
}