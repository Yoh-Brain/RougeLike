using System.Collections.Generic;
using UnityEngine;

public class HextechManager : MonoBehaviour
{
    [Header("引用配置")]
    public SkillManager skillManager;       // 玩家的技能管理器
    public HextechUI hextechUI;             // 海克斯 UI 面板

    [Header("海克斯技能池 (把你的技能资产都放进来)")]
    public List<SkillData> skillPool = new List<SkillData>();

    // 测试用：按 T 键触发抽卡（有了 DebugManager 后这行可以注释掉）
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TriggerHextech();
        }
    }

    public void TriggerHextech()
    {
        if (skillPool.Count == 0) return;

        // 1. 过滤出可以抽取的技能（未满级）
        List<SkillData> availableSkills = new List<SkillData>();
        foreach (var skill in skillPool)
        {
            int currentLv = skillManager.GetSkillLevel(skill);
            if (currentLv < skill.maxLevel)
            {
                availableSkills.Add(skill);
            }
        }

        if (availableSkills.Count == 0)
        {
            Debug.Log("所有技能都已满级，没有可以抽取的技能了！");
            return;
        }

        // 2. 随机抽 3 个（如果池子不足 3 个，就有几个抽几个）
        int drawCount = Mathf.Min(3, availableSkills.Count);
        List<SkillData> choices = GetRandomSkills(availableSkills, drawCount);

        // 3. 让 UI 弹出来展示
        hextechUI.ShowChoices(choices, OnSkillSelected);
    }

    private void OnSkillSelected(SkillData selectedSkill)
    {
        // 直接调用升级/解锁方法（内部会自动判断是升级还是新学）
        bool success = skillManager.UnlockSkill(selectedSkill);
        if (success)
        {
            Debug.Log($"成功获取/升级技能：{selectedSkill.skillName}");
        }
    }

    private List<SkillData> GetRandomSkills(List<SkillData> pool, int count)
    {
        List<SkillData> result = new List<SkillData>();
        List<SkillData> tempPool = new List<SkillData>(pool);

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, tempPool.Count);
            result.Add(tempPool[randomIndex]);
            tempPool.RemoveAt(randomIndex);
        }
        return result;
    }
}