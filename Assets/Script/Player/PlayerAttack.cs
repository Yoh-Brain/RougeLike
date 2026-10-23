using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("攻击范围参数")]
    public float attackRadius = 1.2f;
    public float attackOffsetDistance = 0.8f;
    public float attackCooldown = 0.5f;
    public float attackRecoverTime = 0.8f;
    public LayerMask enemyLayer;

    [Header("攻击时移动速度倍率")]
    [Range(0f, 1f)]
    public float attackMoveSpeedMultiplier = 0.5f;

    [Header("特效设置")]
    public GameObject attackEffectPrefab;
    public float effectLifeTime = 0.5f;

    [Header("引用（留空自动获取）")]
    public Animator animator;
    public PlayerHealth playerHealth;
    public PlayerMovement playerMovement;

    private float lastAttackTime = -999f;
    private bool isAttacking = false;
    private Vector2 attackDirection = Vector2.right;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();
        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) TryAttack();
    }

    public void TryAttack()
    {
        if (playerHealth != null && playerHealth.IsDead) return;
        if (isAttacking) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        isAttacking = true;
        lastAttackTime = Time.time;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 inputDir = new Vector2(h, v);

        if (inputDir.sqrMagnitude > 0.01f)
        {
            attackDirection = inputDir.normalized;
            if (h > 0 && playerMovement.FacingDirection == -1) playerMovement.Flip();
            else if (h < 0 && playerMovement.FacingDirection == 1) playerMovement.Flip();
        }
        else
        {
            attackDirection = new Vector2(playerMovement.FacingDirection, 0f);
        }

        if (playerMovement != null)
        {
            playerMovement.canFlip = false;
            playerMovement.speedMultiplier = attackMoveSpeedMultiplier;
        }

        if (animator != null) animator.SetTrigger("Attack");
        StartCoroutine(AttackRecoverFallback());
    }

    private IEnumerator AttackRecoverFallback()
    {
        yield return new WaitForSeconds(attackRecoverTime);
        if (isAttacking) OnAttackEnd();
    }

    public void OnAttackEffect()
    {
        if (attackEffectPrefab == null) return;
        Vector2 spawnPos = (Vector2)transform.position + attackDirection * attackOffsetDistance;
        GameObject effect = Instantiate(attackEffectPrefab, spawnPos, Quaternion.identity);

        if (attackDirection.x < 0)
        {
            Vector3 scale = effect.transform.localScale;
            scale.x *= -1;
            effect.transform.localScale = scale;
        }
        Destroy(effect, effectLifeTime);
    }

    public void OnAttackHit()
    {
        Vector2 center = (Vector2)transform.position + attackDirection * attackOffsetDistance;
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, attackRadius, enemyLayer);

        foreach (var hit in hits)
        {
            var damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                // 【从属性单例读取】
                float finalDmg = PlayerAttributes.Instance.FinalDamage;
                float finalKnockback = PlayerAttributes.Instance.FinalKnockback;

                damageable.TakeDamage(finalDmg, transform.position, finalKnockback);
            }
        }
    }

    public void OnAttackEnd()
    {
        if (!isAttacking) return;
        isAttacking = false;
        if (playerMovement != null)
        {
            playerMovement.speedMultiplier = 1f;
            playerMovement.canFlip = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 dir = Application.isPlaying ? attackDirection : Vector2.right;
        Vector2 center = (Vector2)transform.position + dir * attackOffsetDistance;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRadius);
    }
}