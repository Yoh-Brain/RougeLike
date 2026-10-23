using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("攻击设置")]
    public float attackCooldown = 1.5f;
    public float attackRange = 1.2f;
    public float attackStopRange = 1.8f;
    public LayerMask playerLayer;

    public bool IsAttacking { get; private set; } = false;
    private float nextAttackTime = 0f;
    private Animator anim;
    private EnemyHealth healthScript;

    void Start()
    {
        anim = GetComponent<Animator>();
        healthScript = GetComponent<EnemyHealth>();
    }

    public void TryAttack()
    {
        if (healthScript != null && healthScript.IsDead) return;
        if (IsAttacking || Time.time < nextAttackTime) return;
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        IsAttacking = true;
        nextAttackTime = Time.time + attackCooldown;
        yield return new WaitForSeconds(0.05f);
        if (anim != null) anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);
        IsAttacking = false;
    }

    // 由动画事件调用
    public void OnAttackHitEvent()
    {
        if (healthScript != null && healthScript.IsDead) return;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackStopRange, playerLayer);

        if (hit != null)
        {
            IDamageable target = hit.GetComponent<IDamageable>();
            if (target != null)
            {
                // 怪物打玩家，不传击退力（默认为0）
                target.TakeDamage(1, transform.position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackStopRange);
    }
}