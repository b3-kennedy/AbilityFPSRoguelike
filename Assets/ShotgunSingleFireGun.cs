using Unity.Netcode;
using UnityEngine;

public class ShotgunSingleFireGun : Gun
{
    public float pelletCount;
    public float spreadAngle;


    public override void Shoot()
    {
        if (!CanShoot()) return;

        if (shootTimer < gunData.fireRate)
        {
            shootTimer += Time.deltaTime;
        }

        if (Input.GetButtonDown("Fire1") && shootTimer >= gunData.fireRate && ammo > 0)
        {
            base.Recoil();
            //anim.SetTrigger("shoot");
            if (GetUseAmmo())
            {
                ammo--;
            }
            GetPlayerInterfaceManager().UpdateAmmoText(ammo);
            shot.Invoke();
            shootTimer -= gunData.fireRate;
            for (int i = 0; i < pelletCount; i++)
            {
                Vector3 direction = GetSpreadDirection(GetCamera().transform.forward, spreadAngle);
                if (Physics.Raycast(GetCamera().transform.position, direction, out RaycastHit hit, 1000f, layerMask))
                {

                    Health health = hit.collider.GetComponent<Health>();
                    UnitData data = hit.collider.GetComponent<UnitData>();
                    Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                    NetworkObject no = hit.collider.GetComponent<NetworkObject>();

                    if (rb && no)
                    {
                        Vector3 dir = (GetCamera().transform.position - rb.position).normalized;
                        ProjectileManager.Instance.AddForceToEnemyServerRpc(hit.collider.GetComponent<NetworkObject>().NetworkObjectId, -dir, 2f, ForceMode.Impulse);
                    }

                    if (health && data.GetTeam() == UnitData.Team.BAD)
                    {
                        Debug.Log("hit");
                        health.TakeDamageServerRpc(gunData.damage);
                    }
                }
            }
        }

    }

    Vector3 GetSpreadDirection(Vector3 forward, float angle)
    {
        return Quaternion.Euler(
            Random.Range(-angle, angle),
            Random.Range(-angle, angle),
            0) * forward;
    }
}
