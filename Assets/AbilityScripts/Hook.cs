using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Hook : Ability
{

    public GameObject hookProjectile;
    Transform firePoint;
    GameObject cam;
    Vector3 hitPoint;
    public LayerMask layerMask;
    public float force;
    Transform hookPoint;

    public override void OnInitialise()
    {
        base.OnInitialise();
        firePoint = GetCaster().transform.Find("CameraHolder/Recoil/Camera/GunPosition/GunParent/Gun/FirePoint");
        hookPoint = GetCaster().transform.Find("Capsule/HookPoint");
        cam = GetCamera();
    }

    public override void PerformCast()
    {
        hitPoint = cam.transform.position + cam.transform.forward * 1000;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 1000, layerMask))
        {
            if (hit.collider)
            {
                hitPoint = hit.point;
            }
        }
        Vector3 dir = (hitPoint - firePoint.position).normalized;
        GameObject spawnedProjectile = Instantiate(hookProjectile, firePoint.position, Quaternion.identity);
        string projectileID = System.Guid.NewGuid().ToString();
        spawnedProjectile.GetComponent<Projectile>().SetValues(force, dir, projectileID);
        spawnedProjectile.GetComponent<HookProjectile>().hookPoint = hookPoint.transform;
        spawnedProjectile.GetComponent<HookProjectile>().player = GetCaster();
        ProjectileManager.Instance.SpawnProjectileServerRpc(NetworkManager.Singleton.LocalClientId, "HookProjectile", firePoint.transform.position, dir, force, projectileID);
    }
}
