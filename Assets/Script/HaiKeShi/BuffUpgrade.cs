using UnityEngine;

[CreateAssetMenu(fileName = "NewBuff", menuName = "Roguelike/Upgrades/BuffUpgrade")]
public class BuffUpgrade : UpgradeData
{
    public StatType statType;
    public float value;
    public ModifierType modifierType;

    public override void Apply()
    {
        // 【修改】直接使用单例
        if (PlayerAttributes.Instance != null)
        {
            PlayerAttributes.Instance.AddModifier(statType, value, modifierType);
        }
        else
        {
            Debug.LogError("找不到 PlayerAttributes 单例！");
        }
    }
}