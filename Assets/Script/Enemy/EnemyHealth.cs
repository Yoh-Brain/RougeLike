using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable, IDieable
{
    [Header("生命值设置")]
    public float maxHealth = 100f;
    public float currentHealth { get; private set; }

    [Header("击退设置")]
    public float knockbackSpeed = 8f;
    public float knockbackDuration = 0.2f;

    public bool IsDead { get; private set; } = false;
    public bool IsKnockedBack { get; private set; } = false;

    // 全局死亡事件，供 EnemySpawner 订阅
    public static event Action OnEnemyDied;

    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        // 如果没有被 Spawner 初始化过，就默认回满血
        if (currentHealth <= 0) currentHealth = maxHealth;
    }

    // 供生成器调用的初始化方法
    public void InitHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = maxHealth;
        IsDead = false;
    }

    public void TakeDamage(float damage, Vector2 sourcePosition)
    {
        if (IsDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} 受到伤害，剩余血量：{currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(KnockbackRoutine(sourcePosition));
        }
    }

    private IEnumerator KnockbackRoutine(Vector2 sourcePosition)
    {
        IsKnockedBack = true;
        if (rb != null)
        {
            Vector2 dir = ((Vector2)transform.position - sourcePosition).normalized;
            rb.velocity = dir * knockbackSpeed;
        }

        yield return new WaitForSeconds(knockbackDuration);

        IsKnockedBack = false;
        if (rb != null) rb.velocity = Vector2.zero;
    }

    public void Die()
    {
        if (IsDead) return;
        IsDead = true;

        Debug.Log($"{gameObject.name} 死亡！广播事件");
        OnEnemyDied?.Invoke(); // 通知生成器补怪

        // 1. 停止移动和物理
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.simulated = false;
        }
        if (col != null) col.enabled = false;

        // 2. 播放死亡动画
        if (anim != null)
        {
            anim.SetTrigger("Die");
            anim.SetBool("IsRunning", false);
        }

        // 3. 让 MonsterBase 去执行掉落物逻辑（解耦）
        MonsterBase baseScript = GetComponent<MonsterBase>();
        if (baseScript != null)
        {
            baseScript.DropLoot();
        }

        // 4. 延迟销毁尸体
        Destroy(gameObject, 1f);
    }
}