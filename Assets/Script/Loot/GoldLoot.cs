using UnityEngine;

public class GoldLoot : BaseLoot
{
    protected override void Collect()
    {
        PlayerStats.AddGold(amount);
    }
}