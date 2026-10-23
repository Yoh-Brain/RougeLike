using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public enum EnemyState { Idle, Running, Attacking }
    public EnemyState currentState = EnemyState.Idle;

    [Header("移动设置")]
    public float speed = 4f;

    private Transform player;
    private Rigidbody2D rb;
    private EnemyHealth healthScript; // 【修改】改为找 EnemyHealth
    private EnemyAttack attackScript;
    private Animator anim;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        healthScript = GetComponent<EnemyHealth>(); // 【修改】
        attackScript = GetComponent<EnemyAttack>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        // 【修改】从 healthScript 里判断死活和击退
        if (healthScript != null && healthScript.IsDead) return;
        if (healthScript != null && healthScript.IsKnockedBack) return;

        if (player == null || healthScript == null || attackScript == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Idle:
            case EnemyState.Running:
                Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, attackScript.attackRange, attackScript.playerLayer);
                if (playerCollider != null)
                {
                    ChangeState(EnemyState.Attacking);
                    attackScript.TryAttack();
                }
                else
                {
                    if (currentState != EnemyState.Running) ChangeState(EnemyState.Running);
                    Vector2 direction = (player.position - transform.position).normalized;
                    rb.velocity = direction * speed;
                    FlipSprite(direction.x);
                }
                break;

            case EnemyState.Attacking:
                rb.velocity = Vector2.zero;
                Vector2 dirToPlayer = (player.position - transform.position).normalized;
                FlipSprite(dirToPlayer.x);
                if (attackScript.IsAttacking) return;
                if (distanceToPlayer > attackScript.attackStopRange)
                {
                    ChangeState(EnemyState.Running);
                }
                else
                {
                    attackScript.TryAttack();
                }
                break;
        }
    }

    private void FlipSprite(float xDirection)
    {
        if (sr == null) return;
        if (xDirection > 0.01f) sr.flipX = false;
        else if (xDirection < -0.01f) sr.flipX = true;
    }

    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;

        if (currentState == EnemyState.Attacking && rb != null)
            rb.bodyType = RigidbodyType2D.Dynamic;

        currentState = newState;

        if (currentState == EnemyState.Attacking)
        {
            if (rb != null) { rb.bodyType = RigidbodyType2D.Kinematic; rb.velocity = Vector2.zero; }
            if (anim != null) anim.SetBool("IsRunning", false);
        }
        else if (currentState == EnemyState.Running)
        {
            if (anim != null) anim.SetBool("IsRunning", true);
        }
        else if (currentState == EnemyState.Idle)
        {
            if (anim != null) anim.SetBool("IsRunning", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackScript != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackScript.attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackScript.attackStopRange);
        }
    }
}