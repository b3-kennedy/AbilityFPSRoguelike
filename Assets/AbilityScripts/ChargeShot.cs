using UnityEngine;

public class ChargeShot : Ability
{
    Gun gun;
    float charge = 1f;
    public float baseDamage;
    public float chargeRate;
    GameObject cam;
    public LayerMask layerMask;
    public override void OnInitialise()
    {
        base.OnInitialise();

        charge = 0f;
        gun = GetCaster().GetComponent<PlayerData>().GetGunParent().GetChild(0).GetChild(0).GetComponent<Gun>();
        gun.shotHit.AddListener(Charge);
        gun.reload.AddListener(OnReload);
        cam = GetCamera();
    }

    void OnReload()
    {
        charge = 0f;
    }

    public override void PerformCast()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 1000f, layerMask))
        {
            if (hit.collider)
            {
                Health health = hit.collider.GetComponent<Health>();
                UnitData data = hit.collider.GetComponent<UnitData>();

                if (health && data.GetTeam() == UnitData.Team.BAD)
                {
                    Debug.Log(baseDamage + charge);
                    health.TakeDamageServerRpc(baseDamage + charge, gun.GetCritChance(), gun.GetCritDamageMultiplier());
                }
                else if (hit.collider.GetComponent<ChargeBallProjectile>())
                {
                    hit.collider.GetComponent<ChargeBallProjectile>().AddDamage(baseDamage + charge);
                }


            }
        }
        charge = 0f;
    }

    void Charge()
    {
        charge += chargeRate;
    }
}
