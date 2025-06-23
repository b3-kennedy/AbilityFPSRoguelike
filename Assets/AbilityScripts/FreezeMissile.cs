using Unity.Netcode;
using UnityEngine;

public class FreezeMissile : Ability
{
    public GameObject missile;
    Transform spawn;
    Transform cam;
    public LayerMask mask;
    public GameObject aoeIndicator;
    public float explsionRadius;
    public float damage;
    GameObject spawnedAOEIndicator;



    public override void OnInitialise()
    {
        base.OnInitialise();
        spawn = GetCaster().transform.Find("Capsule/FreezeMissileSpawn");
        cam = GetCamera().transform;

    }

    public override void Aim()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 1000f, mask))
        {

            if (!spawnedAOEIndicator)
            {
                spawnedAOEIndicator = Instantiate(aoeIndicator, hit.point, Quaternion.identity);
                spawnedAOEIndicator.transform.localScale = Vector3.one * explsionRadius * 2;
            }
            else
            {
                spawnedAOEIndicator.transform.position = hit.point;
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
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 1000f, mask))
        {
            
            if (hit.collider)
            {
                GameObject spawnedMissile = Instantiate(missile, spawn.position, Quaternion.Euler(-90f,0,0));
                JetpackMissile missileScript = spawnedMissile.GetComponent<JetpackMissile>();
                missileScript.damage = damage;
                missileScript.radius = explsionRadius;
                missileScript.SetTarget(hit.point);
                missileScript.onCollide.AddListener(Collision);
                ProjectileManager.Instance.SpawnTargetedProjectileServerRpc(NetworkManager.Singleton.LocalClientId, "FreezeMissile", spawn.position, hit.point);
            }
        }
        

        
    }

    void Collision()
    {
        Destroy(spawnedAOEIndicator);
    }
}
