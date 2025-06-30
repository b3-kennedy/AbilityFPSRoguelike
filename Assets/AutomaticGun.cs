using UnityEngine;

public class AutomaticGun : Gun
{
    public override void Shoot()
    {

        if (!CanShoot()) return;

        if (shootTimer < fireRate)
        {
            shootTimer += Time.deltaTime;
        }

        if (Input.GetButton("Fire1") && shootTimer >= fireRate && ammo > 0)
        {

            base.Raycast();
            base.Recoil();
            //anim.SetTrigger("shoot");
            if (GetUseAmmo())
            {
                ammo--;
            }
            GetPlayerInterfaceManager().UpdateAmmoText(ammo);
            base.shot.Invoke();
            shootTimer -= fireRate;
        }
    }
}