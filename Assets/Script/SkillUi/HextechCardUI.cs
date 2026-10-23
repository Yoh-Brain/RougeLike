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

    private UpgradeData currentUpgrade;
    private Action<UpgradeData> onClickCallback;

    public void Setup(UpgradeData upgrade, Action<UpgradeData> callback)
    {
        currentUpgrade = upgrade;
        onClickCallback = callback;

        if (upgrade == null) return;

        if (skillIcon != null) skillIcon.sprite = upgrade.icon;
        if (skillName != null) skillName.text = string.IsNullOrEmpty(upgrade.upgradeName) ? "Unknown" : upgrade.upgradeName;
        if (skillDesc != null) skillDesc.text = "";

        try
        {
            // 如果是技能升级
            if (upgrade is SkillUpgrade skillUpgrade)
            {
                SkillData skill = skillUpgrade.targetSkill;
                if (skill == null)
                {
                    if (skillDesc != null) skillDesc.text = "<color=red>Error: No Skill Data</color>";
                    return;
                }

                int currentLevel = 0;
                SkillManager skillManager = PlayerAttributes.Instance != null ? PlayerAttributes.Instance.GetComponent<SkillManager>() : null;
                if (skillManager != null) currentLevel = skillManager.GetSkillLevel(skill);

                bool isUpgrade = currentLevel > 0;
                int nextLevel = currentLevel + 1;

                if (skillName != null)
                {
                    skillName.text = isUpgrade ? $"{skill.skillName} <color=#FFD700>Lv.{nextLevel}</color>" : skill.skillName;
                }

                if (skillDesc != null)
                {
                    skillDesc.text = (skill.description ?? "No Description") + "\n\n";
                    if (isUpgrade)
                        skillDesc.text += $"<color=#00FF00>Upgrade to Lv.{nextLevel}</color>";
                    else
                        skillDesc.text += $"<color=#00FF00>Unlock New Skill Lv.1</color>";
                }
            }
            // 如果是 Buff
            else if (upgrade is BuffUpgrade buffUpgrade)
            {
                if (skillDesc != null)
                {
                    string buffColor = buffUpgrade.modifierType == ModifierType.Flat ? "#FFD700" : "#00FF00";
                    string sign = buffUpgrade.value > 0 ? "+" : "";
                    string valText = buffUpgrade.modifierType == ModifierType.Flat
                        ? buffUpgrade.value.ToString()
                        : (buffUpgrade.value * 100).ToString() + "%";

                    // statType.ToString() 会直接输出 Damage, MoveSpeed 等英文
                    skillDesc.text = (upgrade.description ?? "") + $"\n\n<color={buffColor}>{sign}{valText} {buffUpgrade.statType}</color>";
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Card UI Error: {e.Message}");
            if (skillDesc != null) skillDesc.text = "<color=red>Card Data Error</color>";
        }
    }

    public void OnCardClicked()
    {
        onClickCallback?.Invoke(currentUpgrade);
    }
}