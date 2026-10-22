using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("属性")]
    public float speed = 15f;
    public float lifetime = 4f;

    // 伤害不再是固定的，而是由发射者赋予
    private float damage;

    [Header("层级过滤")]
    public LayerMask enemyLayerMask;

    // 【新增】初始化方法，生成箭矢后立刻调用
    public void Setup(float newDamage)
    {
        damage = newDamage;
    }

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = transform.right * speed;
        }
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 层级过滤
        if (((1 << collision.gameObject.layer) & enemyLayerMask) == 0)
        {
            return;
        }

        // 2. 获取 IDamageable 接口并造成伤害
        IDamageable target = collision.GetComponent<IDamageable>();
        if (target != null)
        {
            // 3. 传入伤害和箭矢当前位置作为击退源
            target.TakeDamage(damage, transform.position);
            Destroy(gameObject);
        }
    }
}