using UnityEngine;

public enum SkillType
{
    Projectile,   // 发射箭矢等投射物
    OrbitMelee    // 环绕近战武器
}

[CreateAssetMenu(fileName = "NewSkillData", menuName = "Skills/Skill Data")]
public class SkillData : ScriptableObject
{
    public Sprite icon;

    [Header("基础信息")]
    public string skillName = "多重射击";

    [TextArea(3, 5)]
    public string description = "在这里填写技能描述。";

    public SkillType skillType = SkillType.Projectile;

    [Header("基础数值 (Level 1)")]
    public float baseCooldownTime = 2f;
    public float baseDamage = 20f;
    public int baseArrowsPerShot = 1; // 1级射出1只箭
    public GameObject arrowPrefab;

    [Header("多重射击设置")]
    public float totalSpreadAngle = 30f;

    [Header("成长设置 (每次升级)")]
    public int maxLevel = 5;
    public float damageGrowthPerLevel = 5f; // 每级增加多少伤害
    public int arrowsGrowthPerLevel = 2;    // 每级增加多少只箭 (1级1发，2级3发，3级5发)
    public float cooldownReductionPerLevel = 0.1f; // 每级减少多少冷却

    [Header("环绕近战设置 (skillType = OrbitMelee 时使用)")]
    public GameObject orbitWeaponPrefab;
    public float orbitRadius = 0.8f;
    public float orbitRotationSpeed = 180f;
    public float orbitDamageInterval = 0.5f;
    public float orbitDuration = 0f;
    public LayerMask enemyLayer;

    // --- 【核心封装】：由传入的 level 决定最终的返回值 ---

    public float GetFinalDamage(int level)
    {
        if (level <= 0) level = 1;
        return baseDamage + (level - 1) * damageGrowthPerLevel;
    }

    public int GetFinalArrows(int level)
    {
        if (level <= 0) level = 1;
        return baseArrowsPerShot + (level - 1) * arrowsGrowthPerLevel;
    }

    public float GetFinalCooldown(int level)
    {
        if (level <= 0) level = 1;
        float finalCd = baseCooldownTime - (level - 1) * cooldownReductionPerLevel;
        return Mathf.Max(0.2f, finalCd); // 最低冷却不低于 0.2 秒
    }
}