using UnityEngine;

[CreateAssetMenu(fileName = "NewSkillUpgrade", menuName = "Roguelike/Upgrades/SkillUpgrade")]
public class SkillUpgrade : UpgradeData
{
    public SkillData targetSkill;

    public override void Apply()
    {
        if (PlayerAttributes.Instance == null) return;

        // 【修改】通过玩家单例，获取他身上的 SkillManager 组件
        SkillManager skillManager = PlayerAttributes.Instance.GetComponent<SkillManager>();

        if (skillManager != null && targetSkill != null)
        {
            skillManager.UnlockSkill(targetSkill);
        }
        else
        {
            Debug.LogWarning("未找到 SkillManager 或 targetSkill 为空");
        }
    }
}