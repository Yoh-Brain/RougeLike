using System;
using System.Collections.Generic;
using UnityEngine;

public class HextechUI : MonoBehaviour
{
    [Header("UI 引用")]
    public GameObject panelRoot;
    public GameObject cardPrefab;
    public Transform cardContainer;

    private Action<UpgradeData> onSelectCallback;
    private List<GameObject> spawnedCards = new List<GameObject>();

    void Start()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    public void ShowChoices(List<UpgradeData> choices, Action<UpgradeData> callback)
    {
        onSelectCallback = callback;

        // 【安全气囊1】检查 Panel Root
        if (panelRoot == null)
        {
            Debug.LogError("【UI报错】HextechUI 的 Panel Root 槽位为空！请把面板拖进去！");
            Time.timeScale = 1f; // 强行恢复时间
            return;
        }

        // 【安全气囊2】检查 Card Prefab
        if (cardPrefab == null || cardContainer == null)
        {
            Debug.LogError("【UI报错】HextechUI 的 Card Prefab 或 Card Container 槽位为空！");
            Time.timeScale = 1f;
            return;
        }

        panelRoot.SetActive(true);

        foreach (var card in spawnedCards)
        {
            if (card != null) Destroy(card);
        }
        spawnedCards.Clear();

        foreach (var upgrade in choices)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardContainer);
            spawnedCards.Add(cardObj);

            HextechCardUI cardUI = cardObj.GetComponent<HextechCardUI>();
            if (cardUI != null)
            {
                cardUI.Setup(upgrade, HandleCardSelected);
            }
            else
            {
                Debug.LogError("【UI报错】你的卡牌预制体上没有挂载 HextechCardUI 脚本！");
            }
        }
    }

    private void HandleCardSelected(UpgradeData selectedUpgrade)
    {
        onSelectCallback?.Invoke(selectedUpgrade);
        panelRoot.SetActive(false);
    }
}