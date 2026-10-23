using UnityEngine;

public abstract class UpgradeData : ScriptableObject
{
    [Header("UI 显示信息")]
    public string upgradeName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("抽取权重")]
    public float baseWeight = 10f;

    // 【修改】直接无参，内部直接调用单例
    public abstract void Apply();
}