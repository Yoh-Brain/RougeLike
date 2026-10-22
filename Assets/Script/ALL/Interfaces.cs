using UnityEngine;

// 1. 可受伤接口
public interface IDamageable
{
    void TakeDamage(float damage, Vector2 sourcePosition);
}

// 2. 可死亡接口
public interface IDieable
{
    void Die();
}

// 3. 可攻击接口（可选，如果其他系统需要统一调用）
public interface IAttacker
{
    void TryAttack();
}