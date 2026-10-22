using UnityEngine;

public class LandMinion : MeleeMonster
{
    protected override void Awake()
    {
        base.Awake();

        // 可以在代码里覆写基础属性
        maxHealth = 30f;
        currentHealth = maxHealth;

        // 【新增】在这里初始化特定怪物的掉落数据
        // （或者你直接在 Inspector 面板上改也行，这里只是为了演示代码怎么控制）
        xpAmount = 15;
        goldAmountMin = 1;
        goldAmountMax = 3;
        goldDropChance = 0.8f; // 这个小怪掉钱概率高（80%）
    }
}