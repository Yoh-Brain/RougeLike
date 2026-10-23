using UnityEngine;

// 近战类派系
public abstract class MeleeMonster : MonsterBase
{
    protected virtual void Start()
    {
        // 近战怪通常受重力影响 (直接获取组件，不再依赖 MonsterBase)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.gravityScale = 1f;
    }
}

// 飞行类派系
public abstract class FlyingMonster : MonsterBase
{
    protected virtual void Start()
    {
        // 飞行怪通常忽略重力和地形碰撞
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.gravityScale = 0f;
    }
}