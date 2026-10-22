using System.Collections;
using UnityEngine;

// 继承 MonoBehaviour，并实现 IDieable 和 IDamageable 接口
public class PlayerHealth : MonoBehaviour, IDieable, IDamageable
{
    [Header("生命值设置")]
    public int maxHealth = 5;
    public int CurrentHealth { get; private set; }

    [Header("无敌帧设置")]
    public float invincibleDuration = 0.5f; // 0.5秒无敌
    private bool isInvincible = false;
    private bool isDead = false; // 防止死后重复触发

    private SpriteRenderer sr;
    private Animator anim;

    void Start()
    {
        CurrentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // 【实现 IDamageable 接口】
    public void TakeDamage(float damage, Vector2 sourcePosition)
    {
        if (isInvincible || isDead) return;

        CurrentHealth -= (int)damage;
        if (CurrentHealth < 0) CurrentHealth = 0;

        Debug.Log($"玩家受伤！剩余血量：{CurrentHealth}");

        if (CurrentHealth > 0)
        {
            StartCoroutine(InvincibilityCoroutine());
        }
        else
        {
            Die();
        }
    }

    // 【新增】供调试面板或吃药等外部调用的回血方法
    public void HealToFull()
    {
        if (isDead) return; // 死了不能回血

        CurrentHealth = maxHealth;
        Debug.Log($"玩家已回满血！当前血量：{CurrentHealth}");
    }

    // 【新增】供调试面板直接设置血量（比如测试剩1滴血）
    public void DebugSetHealth(int amount)
    {
        if (isDead) return;

        CurrentHealth = Mathf.Clamp(amount, 0, maxHealth);
        Debug.Log($"调试修改血量：{CurrentHealth}");

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    // 【实现 IDieable 接口】
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("玩家死亡！播放死亡动画");

        if (anim != null) anim.SetTrigger("Die");

        var movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;

        var skillManager = GetComponent<SkillManager>();
        if (skillManager != null) skillManager.enabled = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("1秒已到，销毁玩家对象。");
        Destroy(gameObject);
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        float timer = 0f;
        WaitForSeconds blinkDelay = new WaitForSeconds(0.1f);

        while (timer < invincibleDuration)
        {
            if (sr != null)
            {
                Color c = sr.color;
                c.a = c.a == 1f ? 0.3f : 1f;
                sr.color = c;
            }
            yield return blinkDelay;
            timer += 0.1f;
        }

        if (sr != null)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }
        isInvincible = false;
    }
}