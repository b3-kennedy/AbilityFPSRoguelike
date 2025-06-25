using Unity.Netcode;
using UnityEngine;

public class Vacuum : Ability
{
    Transform firePoint;
    public GameObject projectile;
    public float projectileForce = 25f;
    Transform cam;
    public LayerMask layerMask;
    Vector3 hitPoint;
    public float explosionRadius;
    public float explosionForce;
    PlayerAbilities abilities;
    public GameObject aoeIndicator;
    GameObject spawnedAOEIndicator;
    int altValue = 1;

    public override void OnInitialise()
    {
        base.OnInitialise();
        firePoint = GetCaster().transform.Find("CameraHolder/Recoil/Camera/GunPosition/GunParent/Gun/FirePoint");
        cam = GetCamera().transform;
        abilities = GetCaster().GetComponent<PlayerAbilities>();

    }

    public override void Aim()
    {
        hitPoint = cam.transform.position + cam.transform.forward * 1000;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 1000, layerMask))
        {
            if (hit.collider)
            {
                hitPoint = hit.point;

                if (!spawnedAOEIndicator)
                {
                    spawnedAOEIndicator = Instantiate(aoeIndicator, hit.point, Quaternion.identity);
                    spawnedAOEIndicator.transform.localScale = Vector3.one * explosionRadius * 2;
                }
                else
                {
                    spawnedAOEIndicator.transform.position = hit.point;
                }

            }
        }
        else
        {
            if (spawnedAOEIndicator)
            {
                Destroy(spawnedAOEIndicator);
            }
        }



    }

    public override void PerformCast()
    {
        GameObject spawnedProjectile = Instantiate(projectile, firePoint.position, Quaternion.identity);
        Vector3 dir = (hitPoint - firePoint.position).normalized;

        string projectileID = System.Guid.NewGuid().ToString();

        spawnedProjectile.GetComponent<Projectile>().SetValues(projectileForce, dir, projectileID);

        var altSwitch = abilities.GetAbilityByName("JetpackSwitch");

        if (altSwitch is JetpackAltAbilitySwitch switchScript)
        {
            if (!switchScript.isAlt)
            {
                spawnedProjectile.GetComponent<VacuumProjectile>().altValue = 1;
                ProjectileManager.Instance.SpawnProjectileServerRpc(NetworkManager.Singleton.LocalClientId, "VacuumProjectile", firePoint.position, dir, projectileForce, projectileID);
            }
            else
            {
                spawnedProjectile.GetComponent<VacuumProjectile>().altValue = -1;
                ProjectileManager.Instance.SpawnProjectileServerRpc(NetworkManager.Singleton.LocalClientId, "AltVacuumProjectile", firePoint.position, dir, projectileForce, projectileID);
            }
        }


        spawnedProjectile.GetComponent<VacuumProjectile>().onCollide.AddListener(Collide);


    }

    void Collide()
    {
        Destroy(spawnedAOEIndicator);
    }
}
