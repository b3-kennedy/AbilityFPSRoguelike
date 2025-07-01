using UnityEngine;

public class IncreaseCritChance : UpgradeEffect
{
    public override void Apply()
    {
        int newCritChance = GetGun().gunData.critChance + 10 * count;
        GetGun().SetCritChance(newCritChance);
    }

    public override void Remove()
    {
        GetGun().SetCritChance(0);
    }
}
