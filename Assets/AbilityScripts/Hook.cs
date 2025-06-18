using UnityEngine;

public class Hook : Ability
{

    public GameObject hookProjectile;
    Transform firePoint;

    public override void OnInitialise()
    {
        base.OnInitialise();
        firePoint = GetCaster().transform.Find("CameraHolder/Recoil/Camera/GunPosition/GunParent/Gun/FirePoint");
    }

    public override void PerformCast()
    {
        
    }
}
