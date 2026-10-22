using UnityEngine;

// 静态类，不需要挂载到任何物体上，随程序启动而加载
public static class PlayerStats
{
    // 静态属性，可读可写，与程序共生死
    public static int Gold { get; private set; } = 0;

    // 增加金币的方法（带事件，方便以后 UI 更新）
    public static System.Action<int> OnGoldChanged;

    public static void AddGold(int amount)
    {
        if (amount <= 0) return;

        Gold += amount;

        // 如果 UI 监听了这个事件，就会自动刷新
        OnGoldChanged?.Invoke(Gold);

        Debug.Log($"当前总金币：{Gold}");
    }

    // 消耗金币的方法（为以后的商店功能准备）
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
}