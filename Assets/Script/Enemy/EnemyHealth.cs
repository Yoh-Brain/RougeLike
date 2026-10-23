using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable, IDieable
{
    [Header("生命值设置")]
    public float maxHealth = 100f;
    public float currentHealth { get; private set; }

    [Header("击退设置")]
    public float knockbackDuration = 0.2f; // 击退持续时间

    public bool IsKnockedBack { get; private set; } = false;
    public bool IsDead { get; private set; } = false;

    // 全局死亡事件，供 EnemySpawner/WaveSpawner 订阅
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
        if (currentHealth <= 0) currentHealth = maxHealth;
    }

    public void InitHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = maxHealth;
        IsDead = false;
    }

    // 实现带击退力的受击接口
    public void TakeDamage(float damage, Vector2 sourcePosition, float knockbackForce = 0f)
    {
        if (IsDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} 受到伤害，剩余血量：{currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (knockbackForce > 0f)
        {
            // 传入玩家给的击退力
            StartCoroutine(KnockbackRoutine(sourcePosition, knockbackForce));
        }
    }

    private IEnumerator KnockbackRoutine(Vector2 sourcePosition, float knockbackForce)
    {
        IsKnockedBack = true;
        if (rb != null)
        {
            Vector2 dir = ((Vector2)transform.position - sourcePosition).normalized;
            rb.velocity = dir * knockbackForce;
        }

        yield return new WaitForSeconds(knockbackDuration);

        IsKnockedBack = false;
        if (rb != null) rb.velocity = Vector2.zero;
    }

    public void Die()
    {
        if (IsDead) return;
        IsDead = true;

        OnEnemyDied?.Invoke(); // 通知刷怪器

        if (rb != null) { rb.velocity = Vector2.zero; rb.simulated = false; }
        if (col != null) col.enabled = false;
        if (anim != null) { anim.SetTrigger("Die"); anim.SetBool("IsRunning", false); }

        // 执行掉落
        MonsterBase baseScript = GetComponent<MonsterBase>();
        if (baseScript != null) baseScript.DropLoot();

        Destroy(gameObject, 1f);
    }
}