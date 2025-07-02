using UnityEngine;

public class TrickAuto : Ability
{
    Gun gun;
    public float ammoBonus;

    public override void OnInitialise()
    {
        base.OnInitialise();
        gun = GetGun();
    }

    public override void PerformCast()
    {
        gun.SetAmmo(10);
    }
}
