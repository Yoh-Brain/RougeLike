using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Inventory 
{
    private Item[] items = new Item[5];
   
    public Item this[int index]
    {
        get
        {
            if(index>=0 && index<items.Length)
            {
                return items[index];
            }
            Debug.LogWarning($"背包越界警告！你试图读取第 {index} 格");
            return null;
        }
        set
        {
            if(index>=0&&index<items.Length)
            {
                items[index] = value;
            }
            else
            {
                Debug.LogWarning($"背包越界警告！你试图把物品塞进不存在的第 {index} 格");
            }

        }
    }

    // 自动找空位放物品的逻辑
    public bool AddItem(Item newItem)
    {
        // 遍历背包的 5 个格子
        for (int i = 0; i < items.Length; i++)
        {
            // 这里巧妙使用了索引器的 get！去查这个格子是不是空的
            if (this[i] == null)
            {
                // 如果是空的，就通过索引器的 set 把新物品放进去
                this[i] = newItem;
                Debug.Log($"成功将 {newItem.itemName} 放入第 {i} 格背包");
                return true;
            }
        }

        Debug.Log("背包满了，放不下了！");
        return false;
    }
}
