using UnityEngine;

public class ExpLoot : BaseLoot
{
    protected override void Collect()
    {
        PlayerStats.AddExp(amount);
    }
}