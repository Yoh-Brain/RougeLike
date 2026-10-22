using UnityEngine;
using UnityEngine.UI; // 引入UI命名空间

public class PlayerUI : MonoBehaviour
{
    public PlayerHealth pb;

    [Header("红心设置")]
    public Image[] hearts; // 存放所有红心图片的数组
    public Sprite fullHeart;  // 满血的图片
    public Sprite emptyHeart; // 空血的图片

    void Update()
    {
        if (pb == null || hearts == null) return;

     
        // 遍历所有红心图片
        for (int i = 0; i < hearts.Length; i++)
        {
            // 因为数组是从0开始的，所以判断条件用 i < 当前血量
            if (i < pb.CurrentHealth)
            {
                // 当前索引小于血量，显示满心
                hearts[i].sprite = fullHeart;
                hearts[i].enabled = true; // 确保图片显示
            }
            else
            {
                // 如果血量上限减少了，可以选择显示空心，或者直接隐藏
                // 这里采用显示空心（如果不需要空心，改成 hearts[i].enabled = false; 即可）
                hearts[i].sprite = emptyHeart;
                hearts[i].enabled = true;
            }
        }
    }
}