using System;
using System.Collections.Generic;
using UnityEngine;

public class HextechUI : MonoBehaviour
{
    [Header("UI 引用")]
    public GameObject panelRoot;        // 整个面板（包含三张卡牌），用于显示/隐藏
    public GameObject cardPrefab;       // 刚才做的单张卡牌预制体
    public Transform cardContainer;     // 父节点 (Horizontal Layout Group)

    [Header("数据引用")]
    public SkillManager skillManager;   // 【新增】需要它来查询当前技能等级

    private Action<SkillData> onSelectCallback;
    private List<GameObject> spawnedCards = new List<GameObject>();

    void Start()
    {
        if (panelRoot != null) panelRoot.SetActive(false); // 初始隐藏
    }

    // 供 HextechManager 调用的展示方法
    public void ShowChoices(List<SkillData> choices, Action<SkillData> callback)
    {
        onSelectCallback = callback;
        panelRoot.SetActive(true);

        // 清理上一次生成的旧卡牌
        foreach (var card in spawnedCards) Destroy(card);
        spawnedCards.Clear();

        // 循环生成 3 张新卡牌
        foreach (var skill in choices)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardContainer);
            spawnedCards.Add(cardObj);

            // 获取卡牌上的脚本并注入数据
            HextechCardUI cardUI = cardObj.GetComponent<HextechCardUI>();
            if (cardUI != null)
            {
                // 【核心修复】：查询当前等级，并一起传给 Setup
                int currentLevel = 0;
                if (skillManager != null)
                {
                    currentLevel = skillManager.GetSkillLevel(skill);
                }

                cardUI.Setup(skill, currentLevel, HandleCardSelected);
            }
        }
    }

    // 单张卡牌被点击后的统一处理
    private void HandleCardSelected(SkillData selectedSkill)
    {
        // 把选中的技能传回给 HextechManager
        onSelectCallback?.Invoke(selectedSkill);

        // 隐藏面板
        panelRoot.SetActive(false);
    }
}