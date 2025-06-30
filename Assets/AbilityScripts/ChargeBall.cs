using Unity.Netcode;
using UnityEngine;

public class ChargeBall : Ability
{
    Gun gun;
    public LayerMask layerMask;
    public GameObject ball;
    public float force;
    public float radius;
    GameObject spawnedBall;
    Transform firePoint;
    Vector3 hitPoint;
    GameObject cam;
    PlayerAbilities playerAbilities;
    public override void OnInitialise()
    {
        base.OnInitialise();
        gun = GetGun();
        gun.shot.AddListener(OnShot);
        firePoint = GetCaster().transform.Find("CameraHolder/Recoil/Camera/GunPosition/GunParent/Gun/FirePoint");
        cam = GetCamera();
        playerAbilities = GetCaster().GetComponent<PlayerAbilities>();
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

        spawnedBall = Instantiate(ball, firePoint.position, Quaternion.identity);
        spawnedBall.GetComponent<ChargeBallProjectile>().SetPlayer(GetCaster());
        spawnedBall.GetComponent<ChargeBallProjectile>().SetRadius(radius);
        Vector3 direction = (hitPoint - firePoint.transform.position).normalized;
        string projectileID = System.Guid.NewGuid().ToString();
        spawnedBall.GetComponent<Projectile>().SetValues(force, direction, projectileID);
        ProjectileManager.Instance.SpawnProjectileServerRpc(NetworkManager.Singleton.LocalClientId, "ChargeBallProjectile", firePoint.position, direction, force, projectileID);
        Ability detonate =  playerAbilities.SwitchAbilities("ChargeBall", "DetonateChargeBall");
        if(detonate is DetonateChargeBall detonateChargeBallScript)
        {
            detonateChargeBallScript.SetChargeBall(spawnedBall);
        }

    }

    void OnShot()
    {
        if (Physics.Raycast(GetCamera().transform.position, GetCamera().transform.forward, out RaycastHit hit, 1000, layerMask))
        {
            ChargeBallProjectile projectile = hit.collider.GetComponent<ChargeBallProjectile>();
            if (projectile)
            {
                projectile.AddDamage(gun.gunData.damage);
            }
        }
    }
}
