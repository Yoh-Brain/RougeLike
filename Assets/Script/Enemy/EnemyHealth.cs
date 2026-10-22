using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDieable // 【1】实现接口
{
    [Header("生命值设置")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("击退设置")]
    public float knockbackSpeed = 8f;
    public float knockbackDuration = 0.2f;

    public bool IsKnockedBack { get; private set; } = false;
    private bool isDead = false; // 【新增】防重复死亡

    private Rigidbody2D rb;
    private Animator anim; // 【新增】获取动画机（如果你的敌人也有死亡动画）

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage, Vector2 sourcePosition)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} 受到伤害！剩余血量: {currentHealth}");

        Vector2 knockbackDir = ((Vector2)transform.position - sourcePosition).normalized;
        StartCoroutine(KnockbackRoutine(knockbackDir));

        if (currentHealth <= 0)
        {
            Die(); // 【2】调用接口
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} 死亡！");

        // 1. 停止AI和移动逻辑
        var movement = GetComponent<EnemyMovement>();
        if (movement != null) movement.enabled = false;

        // 2. 播放死亡动画（如果有的话）
        if (anim != null) anim.SetTrigger("Dead");

        // 3. 禁用碰撞体，防止死后挡住玩家的路
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 4. 如果只是简单的敌人，直接销毁
        // Destroy(gameObject);

        // 如果有死亡动画，延迟销毁
        Destroy(gameObject, 1f);
    }

    private IEnumerator KnockbackRoutine(Vector2 dir)
    {
        IsKnockedBack = true;
        if (rb != null) rb.velocity = dir * knockbackSpeed;
        yield return new WaitForSeconds(knockbackDuration);
        IsKnockedBack = false;
        if (rb != null) rb.velocity = Vector2.zero;
    }
}