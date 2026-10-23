using UnityEngine;

public abstract class MonsterBase : MonoBehaviour
{
    [Header("掉落物设置 (通用配置)")]
    public GameObject xpPrefab;
    public int xpAmount = 10;
    public GameObject goldPrefab;
    public int goldAmountMin = 1;
    public int goldAmountMax = 5;
    [Range(0f, 1f)]
    public float goldDropChance = 0.5f;

    // 这个方法由 EnemyHealth.Die() 来调用
    public virtual void DropLoot()
    {
        // 掉落经验
        if (xpPrefab != null)
        {
            GameObject xpObj = Instantiate(xpPrefab, transform.position, Quaternion.identity);
            LootItem xpItem = xpObj.GetComponent<LootItem>();
            if (xpItem != null) xpItem.amount = xpAmount;
        }

        // 掉落金币
        if (goldPrefab != null && Random.value <= goldDropChance)
        {
            int dropAmount = Random.Range(goldAmountMin, goldAmountMax + 1);
            Vector2 randomOffset = Random.insideUnitCircle * 0.3f;
            Vector3 spawnPos = transform.position + (Vector3)randomOffset;
            GameObject goldObj = Instantiate(goldPrefab, spawnPos, Quaternion.identity);
            LootItem goldItem = goldObj.GetComponent<LootItem>();
            if (goldItem != null) goldItem.amount = dropAmount;
        }
    }
}