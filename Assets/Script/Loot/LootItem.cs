using UnityEngine;

public enum LootType { Experience, Gold }

public class LootItem : MonoBehaviour
{
    public LootType lootType;
    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 确保碰到的是带有 Player 标签的物体
        if (collision.CompareTag("Player"))
        {
            if (lootType == LootType.Experience)
            {
                Debug.Log($"拾取了 {amount} 点经验！");
                // 以后可以写 PlayerStats.AddExp(amount);
            }
            else if (lootType == LootType.Gold)
            {
                // 【核心】：向静态类增加金币
                PlayerStats.AddGold(amount);
            }

            // 拾取后销毁自己
            Destroy(gameObject);
        }
    }
}