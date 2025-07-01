using Unity.Netcode;
using UnityEngine;

public class ProjectileSingleFireGun : Gun
{

    public Transform firePoint;
    public GameObject projectile;
    public float shootForce;

    public override void Shoot()
    {

        if (!CanShoot()) return;

        if (shootTimer < fireRate)
        {
            shootTimer += Time.deltaTime;
        }

        if (Input.GetButtonDown("Fire1") && shootTimer >= fireRate && ammo > 0)
        {
            GameObject spawnedProjectile = Instantiate(projectile, firePoint.position, Quaternion.identity);
            
            Vector3 hitPoint = ProjectileRaycast();
            Vector3 dir = (hitPoint - firePoint.position).normalized;
            string projectileID = System.Guid.NewGuid().ToString();
            spawnedProjectile.GetComponent<Projectile>().SetValues(shootForce, dir, projectileID);
            int randomNum = Random.Range(1, 101);
            spawnedProjectile.GetComponent<JetpackProjectile>().SetDamage(gunData.damage, this);

            ProjectileManager.Instance.SpawnProjectileServerRpc(NetworkManager.Singleton.LocalClientId, "JetpackBaseProjectile", firePoint.position, dir, shootForce, projectileID);
            base.Recoil();
            //anim.SetTrigger("shoot");
            if (GetUseAmmo())
            {
                ammo--;
            }
            
            GetPlayerInterfaceManager().UpdateAmmoText(ammo);
            shootTimer -= fireRate;
        }
    }
}

