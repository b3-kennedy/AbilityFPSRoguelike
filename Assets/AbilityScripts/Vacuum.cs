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
    int altValue = 1;

    public override void OnInitialise()
    {
        base.OnInitialise();
        firePoint = GetCaster().transform.Find("CameraHolder/Recoil/Camera/GunPosition/GunParent/Gun/FirePoint");
        cam = GetCaster().transform.Find("CameraHolder/Recoil/Camera");
        abilities = GetCaster().GetComponent<PlayerAbilities>();

    }

    public override void Cast()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 1000, layerMask))
        {
            if (hit.collider) 
            {
                hitPoint = hit.point;
            }
            else
            {
                hitPoint = cam.transform.forward * 1000;
            }
        }

        GameObject spawnedProjectile = Instantiate(projectile, firePoint.position, Quaternion.identity);
        Vector3 dir = (hitPoint - firePoint.position).normalized;
        spawnedProjectile.GetComponent<Projectile>().SetValues(projectileForce, dir);

        var altSwitch = abilities.GetAbilityByName("JetpackSwitch");

        if (altSwitch is JetpackAltAbilitySwitch switchScript)
        {
            if (!switchScript.isAlt)
            {
                spawnedProjectile.GetComponent<VacuumProjectile>().altValue = 1;
                ProjectileManager.Instance.SpawnProjectileServerRpc(NetworkManager.Singleton.LocalClientId, "VacuumProjectile", firePoint.position, dir, projectileForce);
            }
            else
            {
                spawnedProjectile.GetComponent<VacuumProjectile>().altValue = -1;
                ProjectileManager.Instance.SpawnProjectileServerRpc(NetworkManager.Singleton.LocalClientId, "AltVacuumProjectile", firePoint.position, dir, projectileForce);
            }
        }

        



    }
}
