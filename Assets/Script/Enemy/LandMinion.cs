using UnityEngine;

public class LandMinion : MeleeMonster
{
    protected override void Start()
    {
        base.Start(); // 调用 MeleeMonster 里的 Start（设置重力）

        // 血量已经不需要在这里设置了，完全交给 EnemyHealth 和 Spawner 统一调度

        // 在这里初始化特定怪物的掉落数据
        xpAmount = 15;
        goldAmountMin = 1;
        goldAmountMax = 3;
        goldDropChance = 0.8f; // 这个小怪掉钱概率高（80%）
    }
}