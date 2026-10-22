using System.Collections.Generic;
using UnityEngine;

public class ScytheController : MonoBehaviour
{
    [Header("环绕设置")]
    public Transform orbitCenter;
    public float orbitRadius = 0.8f;
    public float rotationSpeed = 180f;
    public bool clockwise = true;
    public float startAngle = 0f;

    [Header("修正补偿")]
    [Tooltip("如果刀柄没对准玩家，调整这个值（度）。填 -90 或 90 试试")]
    public float rotationOffset = 0f;
    [Tooltip("如果玩家轴心在脚底，填一个正数把镰刀往上提一点")]
    public float centerYOffset = 0.5f;

    [Header("伤害设置")]
    public float damage = 20f;
    public float damageInterval = 0.5f;
    public LayerMask enemyLayer;

    [Header("生命周期")]
    public float duration = 0f;

    private float currentAngle;
    private float lifeTimer;
    private Dictionary<Collider2D, float> lastDamageTime = new Dictionary<Collider2D, float>();

    public void Init(Transform center, float dmg, float interval, float dur, LayerMask layer)
    {
        orbitCenter = center;
        damage = dmg;
        damageInterval = interval;
        duration = dur;
        enemyLayer = layer;
        lifeTimer = 0f;
    }

    private void Start()
    {
        if (orbitCenter == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) orbitCenter = player.transform;
        }
    }

    private void Update()
    {
        if (orbitCenter == null) return;

        // 1. 绕中心旋转
        float dir = clockwise ? -1f : 1f;
        currentAngle += rotationSpeed * dir * Time.deltaTime;

        float rad = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * orbitRadius;

        // 加上 Y 轴偏移，解决绕脚底板转的问题
        Vector3 centerPos = orbitCenter.position;
        centerPos.y += centerYOffset;

        // 设置位置
        transform.position = centerPos + offset;

        // 设置旋转（加入图片朝向补偿）
        transform.rotation = Quaternion.Euler(0f, 0f, currentAngle + rotationOffset);

        // 2. 生命周期
        if (duration > 0f)
        {
            lifeTimer += Time.deltaTime;
            if (lifeTimer >= duration)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // 1. 层级过滤
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        // 2. 伤害间隔检查
        if (lastDamageTime.TryGetValue(other, out float lastTime))
        {
            if (Time.time - lastTime < damageInterval) return;
        }
        lastDamageTime[other] = Time.time;

        // 3. 获取 IDamageable 接口并造成伤害
        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damage, transform.position);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        lastDamageTime.Remove(other);
    }
}