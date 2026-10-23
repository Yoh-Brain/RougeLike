using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
    public static PlayerAttributes Instance { get; private set; }

    [Header("基础属性面板")]
    public float baseDamage = 10f;
    public float baseKnockback = 5f;
    public float baseMoveSpeed = 5f;

    private List<StatModifier> modifiers = new List<StatModifier>();

    public float FinalDamage => CalculateFinalValue(baseDamage, StatType.Damage);
    public float FinalKnockback => CalculateFinalValue(baseKnockback, StatType.Knockback);
    public float FinalMoveSpeed => CalculateFinalValue(baseMoveSpeed, StatType.MoveSpeed);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private float CalculateFinalValue(float baseValue, StatType type)
    {
        float flatBonus = 0f;
        float percentBonus = 0f;

        foreach (var mod in modifiers)
        {
            if (mod.statType == type)
            {
                if (mod.modifierType == ModifierType.Flat) flatBonus += mod.value;
                else if (mod.modifierType == ModifierType.Percent) percentBonus += mod.value;
            }
        }
        return (baseValue + flatBonus) * (1f + percentBonus);
    }

    public void AddModifier(StatType type, float value, ModifierType modifierType)
    {
        modifiers.Add(new StatModifier(type, value, modifierType));
        Debug.Log($"<color=cyan>[属性提升] {type} 增加了 {value} ({modifierType})</color>");
    }
}

public enum StatType { Damage, Knockback, MoveSpeed }
public enum ModifierType { Flat, Percent }

public class StatModifier
{
    public StatType statType;
    public float value;
    public ModifierType modifierType;

    public StatModifier(StatType type, float val, ModifierType modType)
    {
        statType = type;
        value = val;
        modifierType = modType;
    }
}