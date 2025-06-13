using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class ProjectileManager : NetworkBehaviour
{

    public static ProjectileManager Instance;
    Dictionary<string, GameObject> projectiles = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject[] loadedProjectiles = Resources.LoadAll<GameObject>("Projectiles");

        foreach (var item in loadedProjectiles)
        {
            if (!projectiles.ContainsKey(item.name))
            {
                projectiles.Add(item.name, item);
                Debug.Log($"Loaded {item.name}");
            }
            else
            {
                Debug.LogWarning($"Duplicate character key found: {item.name}");
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnProjectileServerRpc(ulong clientID, string projectileName, Vector3 position, Vector3 direction, float force)
    {
        SpawnProjectileClientRpc( clientID,  projectileName,  position,  direction,  force);
    }

    [ClientRpc]
    void SpawnProjectileClientRpc(ulong clientID, string projectileName, Vector3 position, Vector3 direction, float force)
    {
        if (NetworkManager.Singleton.LocalClientId == clientID) return;

        if (projectiles.TryGetValue(projectileName, out var projectile))
        {
            GameObject spawnedProjectile = Instantiate(projectile, position, Quaternion.identity);
            spawnedProjectile.GetComponent<Projectile>().SetValues(force, direction);
            spawnedProjectile.name = projectileName + "Server";

        }
        else
        {
            Debug.Log($"Projectile with name {projectileName} not found");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreateExplosionServerRpc(ulong clientID, Vector3 position, float radius, float force)
    {
        CreateExplosionClientRpc( clientID, position, radius, force);
    }

    [ClientRpc]
    void CreateExplosionClientRpc(ulong clientID, Vector3 position, float radius, float force)
    {
        if (NetworkManager.Singleton.LocalClientId == clientID) return;

        Collider[] colliders = new Collider[50];
        int count = Physics.OverlapSphereNonAlloc(transform.position, radius, colliders);
        for (int i = 0; i < count; i++)
        {
            Rigidbody rb = colliders[i].attachedRigidbody;
            Vector3 dir = (colliders[i].transform.position - transform.position).normalized;
            if (rb)
            {
                rb.AddForce(dir * force, ForceMode.Impulse);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyProjectileServerRpc(ulong objectID)
    {
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectID, out var projectile))
        {
            projectile.Despawn();
        }
    }
}
