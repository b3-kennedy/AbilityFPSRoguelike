using UnityEngine;

public class IncreaseCritChance : UpgradeEffect
{

    public int critChanceIncrease;
    public override void Apply()
    {
        int newCritChance = GetGun().gunData.critChance + critChanceIncrease * count;
        GetGun().SetCritChance(newCritChance);
    }

    public override void OnSpawned()
    {
        base.OnSpawned();
        SetToolTipDescription($"Increase crit chance by {critChanceIncrease}%");
    }

    public override void Remove()
    {
        GetGun().SetCritChance(0);
    }
}
