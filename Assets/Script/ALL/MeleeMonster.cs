using UnityEngine;

// 近战类派系
public abstract class MeleeMonster : MonsterBase
{
    protected override void Awake()
    {
        base.Awake();
        // 近战怪通常受重力影响
        if (rb != null) rb.gravityScale = 1f;
    }
}

// 飞行类派系
public abstract class FlyingMonster : MonsterBase
{
    protected override void Awake()
    {
        base.Awake();
        // 飞行怪通常忽略重力和地形碰撞
        if (rb != null) rb.gravityScale = 0f;
        // 这里可以开启无视差、悬浮动画等
    }
}