using System.Collections.Generic;
using UnityEngine;

public class LevelUpController : MonoBehaviour
{
    [Header("UI 引用")]
    public HextechUI hextechUI; // 把包含三选一面板的物体拖进来

    private void Start()
    {
        // 开始监听玩家的升级事件
        PlayerStats.OnLevelUp += HandleLevelUp;
    }

    private void OnDestroy()
    {
        // 取消监听，防止内存泄漏
        PlayerStats.OnLevelUp -= HandleLevelUp;
    }

    private void HandleLevelUp(int newLevel)
    {
        Debug.Log($"<color=yellow>【接线员】收到升级通知！等级 {newLevel}，准备弹窗...</color>");

        // 1. 抽卡前先检查是否有池子和UI
        if (UpgradeManager.Instance == null || hextechUI == null)
        {
            Debug.LogError("抽卡失败：UpgradeManager 或 HextechUI 引用为空！");
            return;
        }

        // 2. 暂停游戏
        Time.timeScale = 0f;

        // 3. 从 UpgradeManager 抽 3 张卡
        List<UpgradeData> choices = UpgradeManager.Instance.GetThreeRandomUpgrades();

        // 4. 让 UI 弹出来展示
        hextechUI.ShowChoices(choices, OnPlayerSelectedUpgrade);
    }

    private void OnPlayerSelectedUpgrade(UpgradeData selectedUpgrade)
    {
        // 1. 执行选中的效果（加属性或加技能）
        selectedUpgrade.Apply();
        Debug.Log($"<color=green>玩家选择了：{selectedUpgrade.upgradeName}</color>");

        // 2. 恢复游戏时间
        Time.timeScale = 1f;
    }
}