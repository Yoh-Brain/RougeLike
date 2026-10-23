using UnityEngine;

// 1. 可受伤接口
public interface IDamageable
{
    // 【核心修改】增加了 float knockbackForce = 0f，赋予默认值防止旧代码报错
    void TakeDamage(float damage, Vector2 sourcePosition, float knockbackForce = 0f);
}

// 2. 可死亡接口
public interface IDieable
{
    void Die();
}

// 3. 可攻击接口
public interface IAttacker
{
    void TryAttack();
}