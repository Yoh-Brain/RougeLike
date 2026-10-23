using System;
using UnityEngine;

public static class PlayerStats
{
    // ================= 金币系统 =================
    public static int Gold { get; private set; } = 0;
    public static System.Action<int> OnGoldChanged;

    public static void AddGold(int amount)
    {
        if (amount <= 0) return;
        Gold += amount;
        OnGoldChanged?.Invoke(Gold);
        Debug.Log($"当前总金币：{Gold}");
    }

    public static bool SpendGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            OnGoldChanged?.Invoke(Gold);
            return true;
        }
        return false;
    }

    // ================= 经验与等级系统 =================
    public static int CurrentLevel { get; private set; } = 1;
    public static int CurrentExp { get; private set; } = 0;

    // 【核心修改】：用动态数组作为经验表。索引 0 对应 1 级升 2 级，索引 1 对应 2 级升 3 级...
    public static int[] ExpTable = { 5, 15, 30, 50, 80, 120, 170, 230, 300 };

    // 事件声明，与金币保持高度一致
    public static event Action<int, int> OnExpChanged; // 参数：(当前经验, 升级所需经验)
    public static event Action<int> OnLevelUp;

    // 辅助属性：动态获取当前等级所需的经验值
    public static int MaxExpForCurrentLevel
    {
        get
        {
            // 防止等级超过数组上限（满级后所需经验设为极大值或固定值）
            int index = CurrentLevel - 1;
            if (index < ExpTable.Length) return ExpTable[index];
            return ExpTable[ExpTable.Length - 1] + (index - ExpTable.Length + 1) * 100;
        }
    }

    public static void AddExp(int amount)
    {
        if (amount <= 0) return;

        CurrentExp += amount;

        // 循环检查是否升级（支持一次吃多个经验球连升数级）
        while (CurrentExp >= MaxExpForCurrentLevel)
        {
            CurrentExp -= MaxExpForCurrentLevel;
            CurrentLevel++;

            OnLevelUp?.Invoke(CurrentLevel);
            Debug.Log($"<color=yellow>升级！当前等级：{CurrentLevel}</color>");
        }

        OnExpChanged?.Invoke(CurrentExp, MaxExpForCurrentLevel);
    }
}