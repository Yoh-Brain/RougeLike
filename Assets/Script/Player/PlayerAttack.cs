using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("攻击参数")]
    public float attackDamage = 10f;
    public float attackRadius = 1.2f;
    public float attackOffsetDistance = 0.8f;
    public float attackCooldown = 0.5f;
    public float attackRecoverTime = 0.8f;
    public LayerMask enemyLayer;

    [Header("攻击时移动速度倍率")]
    [Range(0f, 1f)]
    public float attackMoveSpeedMultiplier = 0.5f;

    // ================= 新增：特效设置 =================
    [Header("特效设置")]
    public GameObject attackEffectPrefab; // 你的刀光 Prefab 拖到这里
    public float effectLifeTime = 0.5f;   // 刀光存活时间（应 >= 动画长度）
    // ==================================================

    [Header("引用（留空自动获取）")]
    public Animator animator;
    public PlayerHealth playerHealth;
    public PlayerMovement playerMovement;

    private float lastAttackTime = -999f;
    private bool isAttacking = false;
    private Vector2 attackDirection = Vector2.right;

    public bool IsAttacking => isAttacking;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();
        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }
    }

    public void TryAttack()
    {
        if (playerHealth != null && playerHealth.IsDead) return;
        if (isAttacking) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        isAttacking = true;
        lastAttackTime = Time.time;

        // 用 WASD 输入作为攻击向量
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 inputDir = new Vector2(h, v);

        if (inputDir.sqrMagnitude > 0.01f)
        {
            attackDirection = inputDir.normalized;

            // 如果按下攻击时玩家在按反方向，先强行让他转向面对攻击方向
            if (h > 0 && playerMovement.FacingDirection == -1)
            {
                playerMovement.Flip();
            }
            else if (h < 0 && playerMovement.FacingDirection == 1)
            {
                playerMovement.Flip();
            }
        }
        else
        {
            // 如果没按方向键，就朝当前面朝的方向攻击
            attackDirection = new Vector2(playerMovement.FacingDirection, 0f);
        }

        // 锁定翻转，动画不结束不许转身
        if (playerMovement != null)
        {
            playerMovement.canFlip = false;
            playerMovement.speedMultiplier = attackMoveSpeedMultiplier;
        }

        // 触发攻击动画
        if (animator != null) animator.SetTrigger("Attack");

        StartCoroutine(AttackRecoverFallback());
    }

    private IEnumerator AttackRecoverFallback()
    {
        yield return new WaitForSeconds(attackRecoverTime);
        if (isAttacking) OnAttackEnd();
    }

    // ===== Animation Event 接收方法 =====

    // 【新增】绑在动画的特效帧（通常是挥刀那一瞬间）
    public void OnAttackEffect()
    {
        Debug.Log("动画事件触发了！准备生成特效"); // 加上这行
        if (attackEffectPrefab == null)
        {
            Debug.LogError("糟了，Attack Effect Prefab 槽位是空的，没把预制体拖进来！");
            return;
        }


        // 1. 计算生成位置
        Vector2 spawnPos = (Vector2)transform.position + attackDirection * attackOffsetDistance;

        // 2. 生成特效（默认不旋转，保证多帧动画按原始方向播放）
        GameObject effect = Instantiate(attackEffectPrefab, spawnPos, Quaternion.identity);

        // 3. 处理左右翻转（如果攻击方向有向左的分量，水平镜像翻转）
        if (attackDirection.x < 0)
        {
            Vector3 scale = effect.transform.localScale;
            scale.x *= -1;
            effect.transform.localScale = scale;
        }

        // 4. 核心兜底：时间一到强制销毁特效，防止特效卡在场景里
        Destroy(effect, effectLifeTime);
    }

    // 绑在动画的伤害判定帧
    public void OnAttackHit()
    {
        Vector2 center = (Vector2)transform.position + attackDirection * attackOffsetDistance;

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, attackRadius, enemyLayer);
        foreach (var hit in hits)
        {
            var damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage, transform.position);
            }
        }
    }

    // 绑在动画的最后一帧
    public void OnAttackEnd()
    {
        if (!isAttacking) return;
        isAttacking = false;

        // 恢复速度，解锁翻转
        if (playerMovement != null)
        {
            playerMovement.speedMultiplier = 1f;
            playerMovement.canFlip = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 dir = attackDirection;
        if (!Application.isPlaying)
        {
            dir = Vector2.right;
        }
        Vector2 center = (Vector2)transform.position + dir * attackOffsetDistance;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRadius);
    }
}