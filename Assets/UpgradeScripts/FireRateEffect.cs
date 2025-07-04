using UnityEngine;

public class FireRateEffect : UpgradeEffect
{

    public float value;
    
    public override void Apply()
    {
        Gun gun = GetGun();
        float newFireRate = gun.gunData.fireRate * Mathf.Pow(value, count);
        gun.SetFireRate(newFireRate);

    }

    public override void OnSpawned()
    {
        base.OnSpawned();
        SetToolTipDescription($"Increase fire rate by {100 * value} %");
        
    }

    public override void Remove()
    {
        Gun gun = GetGun();
        gun.SetFireRate(gun.gunData.fireRate);
    }
}
