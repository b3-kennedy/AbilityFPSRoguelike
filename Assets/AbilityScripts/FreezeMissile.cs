using Unity.Netcode;
using UnityEngine;

public class FreezeMissile : Ability
{
    public GameObject missile;
    Transform spawn;
    Transform cam;
    public LayerMask mask;



    public override void OnInitialise()
    {
        base.OnInitialise();
        spawn = GetCaster().transform.Find("Capsule/FreezeMissileSpawn");
        cam = GetCamera().transform;

    }

    public override void PerformCast()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 1000f, mask))
        {
            
            if (hit.collider)
            {
                GameObject spawnedMissile = Instantiate(missile, spawn.position, Quaternion.Euler(-90f,0,0));
                JetpackMissile missileScript = spawnedMissile.GetComponent<JetpackMissile>();
                missileScript.SetTarget(hit.point);
                ProjectileManager.Instance.SpawnTargetedProjectileServerRpc(NetworkManager.Singleton.LocalClientId, "FreezeMissile", spawn.position, hit.point);
            }
        }
        

        
    }
}
