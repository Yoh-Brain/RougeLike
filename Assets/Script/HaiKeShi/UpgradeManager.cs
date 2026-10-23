using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; } // 顺便做个单例方便调用

    [Header("所有可用的升级选项 (在Inspector中配置)")]
    public List<UpgradeData> allUpgrades;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 抽取三个不重复的升级选项
    public List<UpgradeData> GetThreeRandomUpgrades()
    {
        List<UpgradeData> availablePool = new List<UpgradeData>(allUpgrades);
        List<UpgradeData> results = new List<UpgradeData>();
        int drawCount = Mathf.Min(3, availablePool.Count);

        for (int i = 0; i < drawCount; i++)
        {
            if (availablePool.Count == 0) break;

            float totalWeight = 0f;
            foreach (var upgrade in availablePool) totalWeight += upgrade.baseWeight;

            float randomPoint = Random.Range(0f, totalWeight);
            float currentSum = 0f;
            int selectedIndex = 0;

            for (int j = 0; j < availablePool.Count; j++)
            {
                currentSum += availablePool[j].baseWeight;
                if (randomPoint <= currentSum)
                {
                    selectedIndex = j;
                    break;
                }
            }

            results.Add(availablePool[selectedIndex]);
            availablePool.RemoveAt(selectedIndex);
        }
        return results;
    }
}