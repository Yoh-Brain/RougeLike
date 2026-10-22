using System.Collections;
using UnityEngine;

public abstract class MonsterBase : MonoBehaviour, IDamageable, IDieable
{
    [Header("基础数据")]
    public float maxHealth = 100f;
    protected float currentHealth;

    public bool IsDead { get; protected set; } = false;
    public bool IsKnockedBack { get; private set; } = false;

    [Header("击退设置")]
    public float knockbackSpeed = 8f;
    public float knockbackDuration = 0.2f;

    [Header("掉落物设置 (通用配置)")]
    public GameObject xpPrefab;             // 经验球预制体
    public int xpAmount = 10;               // 经验数量（100%掉落）

    public GameObject goldPrefab;           // 金币预制体
    public int goldAmountMin = 1;           // 最少掉多少金币
    public int goldAmountMax = 5;           // 最多掉多少金币
    [Range(0f, 1f)]
    public float goldDropChance = 0.5f;     // 金币掉落概率（0.5 = 50%）

    protected Animator anim;
    protected Rigidbody2D rb;
    protected Collider2D col;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage, Vector2 sourcePosition)
    {
        if (IsDead) return;
        currentHealth -= damage;

        if (currentHealth <= 0) Die();
        else OnHurt(sourcePosition);
    }

    protected virtual void OnHurt(Vector2 sourcePosition)
    {
        StartCoroutine(KnockbackRoutine(sourcePosition));
    }

    private IEnumerator KnockbackRoutine(Vector2 sourcePosition)
    {
        IsKnockedBack = true;
        Vector2 knockbackDir = ((Vector2)transform.position - sourcePosition).normalized;
        if (rb != null) rb.velocity = knockbackDir * knockbackSpeed;
        yield return new WaitForSeconds(knockbackDuration);
        IsKnockedBack = false;
        if (rb != null) rb.velocity = Vector2.zero;
    }

    public virtual void Die()
    {
        if (IsDead) return;
        IsDead = true;

        // 1. 播放死亡动画
        if (anim != null)
        {
            anim.SetTrigger("Die");
            anim.SetBool("IsRunning", false); // 防止残留跑步状态
        }

        // 2. 刚体处理：停止所有物理运动，防止尸体滑动或被推挤
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.simulated = false; // 彻底关闭物理模拟，变成真正的尸体
        }

        // 3. 禁用碰撞体，防止死后阻挡子弹
        if (col != null) col.enabled = false;

        // 4. 执行掉落逻辑
        DropLoot();

        // 5. 延迟销毁尸体（给死亡动画留出时间）
        Destroy(gameObject, 1f);
    }

    protected virtual void DropLoot()
    {
        // 必定掉落经验
        if (xpPrefab != null)
        {
            GameObject xpObj = Instantiate(xpPrefab, transform.position, Quaternion.identity);
            LootItem xpItem = xpObj.GetComponent<LootItem>();
            if (xpItem != null) xpItem.amount = xpAmount;
        }

        // 概率掉落金币
        if (goldPrefab != null && Random.value <= goldDropChance)
        {
            int dropAmount = Random.Range(goldAmountMin, goldAmountMax + 1);

            // 稍微加个随机偏移，让金币不要完全堆在一个点上
            Vector2 randomOffset = Random.insideUnitCircle * 0.3f;
            Vector3 spawnPos = transform.position + (Vector3)randomOffset;

            GameObject goldObj = Instantiate(goldPrefab, spawnPos, Quaternion.identity);
            LootItem goldItem = goldObj.GetComponent<LootItem>();
            if (goldItem != null) goldItem.amount = dropAmount;
        }
    }
}