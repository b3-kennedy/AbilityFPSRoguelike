using System;
using Unity.Netcode;
using UnityEngine;

public class BoulderSlam : Ability
{

    public GameObject boulder;
    GameObject spawnedBoulder;
    public GameObject aoeIndicator;
    GameObject spawnedAOEIndicator;
    Vector3 hitPoint;
    public LayerMask layerMask;
    GameObject cam;
    public float radius;
    public float height;
    public float damage;
    Gun gun;
    PlayerAbilities playerAbilities;
    int combo;

    public override void OnInitialise()
    {
        base.OnInitialise();
        cam = GetCamera();
        gun = GetGun();
        gun.shot.AddListener(Shot);
        playerAbilities = GetCaster().GetComponent<PlayerAbilities>();
        combo = 0;
        
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
                    spawnedAOEIndicator.transform.localScale = new Vector3(radius*2f, height, radius*2f);
                }
                else
                {
                    spawnedAOEIndicator.transform.position = hit.point;
                    spawnedAOEIndicator.transform.position = new Vector3(spawnedAOEIndicator.transform.position.x,
                        spawnedAOEIndicator.transform.position.y + spawnedAOEIndicator.transform.localScale.y * 0.9f,
                        spawnedAOEIndicator.transform.position.z);
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

    void Shot()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 1000, layerMask))
        {
            Boulder boulder = hit.collider.GetComponent<Boulder>();
            if (boulder)
            {
                boulder.Activate();
                GetCaster().GetComponent<ActivateBoulderOnServer>().ActivateBoulderServerRpc(boulder.GetComponent<Projectile>().ID);
            }
        }
    }

    public override void PerformCast()
    {
        spawnedBoulder = Instantiate(boulder, new Vector3(hitPoint.x, hitPoint.y + height * 1.8f, hitPoint.z), Quaternion.identity);
        spawnedBoulder.GetComponent<Rigidbody>().isKinematic = true;
        string projectileID = Guid.NewGuid().ToString();
        spawnedBoulder.GetComponent<Projectile>().SetValues(0, Vector3.zero, projectileID);
        Ability trick = playerAbilities.GetAbilityByName("TrickReload");
        if (trick is TrickReload trickReload)
        {
            combo = trickReload.GetCombo();
            if(combo <= 0)
            {
                combo = 1;
            }
        }


        spawnedBoulder.GetComponent<Boulder>().SetValues(damage * combo, radius);
        ProjectileManager.Instance.SpawnProjectileServerRpc(NetworkManager.Singleton.LocalClientId, "Boulder", new Vector3(hitPoint.x, hitPoint.y + height * 1.8f, hitPoint.z), Vector3.zero, 0, projectileID);
        if (spawnedAOEIndicator)
        {
            Destroy(spawnedAOEIndicator);
        }
    }
}
