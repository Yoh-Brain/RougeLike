using UnityEngine;

public class Pickup : MonoBehaviour
{
    public Item itemData;

    private void Start()
    {
        itemData = new Item("红药水", 5);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("1. 有东西碰到我了！碰到的是：" + collision.name); // 如果没看到这句，说明物理设置错了

        if (collision.CompareTag("Player"))
        {
            Debug.Log("2. 碰到的是玩家！"); // 如果没看到这句，说明 Tag 错了

            PlayerMovement player = collision.GetComponent<PlayerMovement>();

            if (player != null)
            {
                Debug.Log("3. 找到玩家身上的 PlayerMovement 脚本了！");

                bool success = player.bag.AddItem(itemData);

                if (success)
                {
                    Debug.Log("4. 成功放进背包，准备销毁！");
                    Destroy(gameObject); // 销毁物体
                }
                else
                {
                    Debug.Log("4. 背包满了，放不进去！");
                }
            }
            else
            {
                Debug.Log("3. 玩家身上没有 PlayerMovement 脚本！");
            }
        }
    }
}