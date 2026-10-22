using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HextechCardUI : MonoBehaviour
{
    [Header("UI 引用")]
    public Image backgroundImage;
    public Image skillIcon;
    public TextMeshProUGUI skillName;
    public TextMeshProUGUI skillDesc;

    private SkillData currentSkill;
    private Action<SkillData> onClickCallback;

    // 【修改点】：需要传入当前等级来计算描述
    public void Setup(SkillData skill, int currentLevel, Action<SkillData> callback)
    {
        currentSkill = skill;
        onClickCallback = callback;

        if (skillIcon != null) skillIcon.sprite = skill.icon;

        // 1. 判断是升级还是新学
        bool isUpgrade = currentLevel > 0;
        int nextLevel = currentLevel + 1;

        // 2. 修改标题
        if (skillName != null)
        {
            if (isUpgrade)
                skillName.text = $"{skill.skillName} <color=#FFD700>Lv.{nextLevel}</color>"; // 升级时显示等级
            else
                skillName.text = skill.skillName;
        }

        // 3. 动态生成描述
        if (skillDesc != null)
        {
            string baseDesc = skill.description + "\n\n";

            if (isUpgrade)
            {
                // 显示升级对比
                int oldArrows = skill.GetFinalArrows(currentLevel);
                int newArrows = skill.GetFinalArrows(nextLevel);
                float oldDmg = skill.GetFinalDamage(currentLevel);
                float newDmg = skill.GetFinalDamage(nextLevel);

                baseDesc += $"<color=#888888>当前：{oldArrows}发 / {oldDmg}伤害</color>\n";
                baseDesc += $"<color=#00FF00>升级：{newArrows}发 / {newDmg}伤害</color>";
            }
            else
            {
                // 新学技能，直接显示基础属性
                baseDesc += $"<color=#00FF00>箭矢数量：{skill.GetFinalArrows(1)}发</color>\n";
                baseDesc += $"<color=#00FF00>基础伤害：{skill.GetFinalDamage(1)}</color>";
            }
            skillDesc.text = baseDesc;
        }
    }

    public void OnCardClicked()
    {
        onClickCallback?.Invoke(currentSkill);
    }
}