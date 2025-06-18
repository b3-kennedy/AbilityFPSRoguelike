using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class ProjectileManager : NetworkBehaviour
{

    public static ProjectileManager Instance;
    Dictionary<string, GameObject> projectiles = new Dictionary<string, GameObject>();

    List<Rigidbody> objectToMove = new List<Rigidbody>();

    public Dictionary<string, GameObject> GetProjectilesDictionary()
    {
        return projectiles;
    }


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
    public void SpawnThrowingKnifeServerRpc(ulong clientID, string projectileName, Vector3 position, Vector3 direction, float force)
    {
        SpawnThrowingKnifeClientRpc(clientID, projectileName, position, direction, force);
    }


    [ClientRpc]
    void SpawnThrowingKnifeClientRpc(ulong clientID, string projectileName, Vector3 position, Vector3 direction, float force)
    {
        if (NetworkManager.Singleton.LocalClientId == clientID) return;

        if (projectiles.TryGetValue(projectileName, out var projectile))
        {
            GameObject spawnedProjectile = Instantiate(projectile, position, Quaternion.identity);
            spawnedProjectile.GetComponent<Projectile>().SetValues(force, direction);
            spawnedProjectile.GetComponent<ExplosiveThrowingKnifeProjectile>().player = NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject.gameObject;
            spawnedProjectile.GetComponent<ExplosiveThrowingKnifeProjectile>().knifeType = ExplosiveThrowingKnifeProjectile.KnifeType.SERVER;
            spawnedProjectile.name = projectileName + "Server";

        }
        else
        {
            Debug.Log($"Projectile with name {projectileName} not found");
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
    public void SpawnTargetedProjectileServerRpc(ulong clientID, string projectileName, Vector3 spawnPosition, Vector3 targetPosition)
    {
        SpawnTargetedProjectileClientRpc(clientID, projectileName, spawnPosition, targetPosition);
    }

    [ClientRpc]
    void SpawnTargetedProjectileClientRpc(ulong clientID, string projectileName, Vector3 spawnPosition, Vector3 targetPosition)
    {
        if (clientID == NetworkManager.Singleton.LocalClientId) return;

        if(projectiles.TryGetValue(projectileName, out var projectile))
        {
            TargetedProjectile targetedProjectile = Instantiate(projectile, spawnPosition, projectile.transform.rotation).GetComponent<TargetedProjectile>();
            targetedProjectile.SetTarget(targetPosition);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void MoveObjectOnServerRpc(ulong objectID, Vector3 dir, float force, Vector3 destination)
    {
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectID, out var obj))
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb)
            {
                if(Vector3.Distance(obj.transform.position, destination) >= 0.5f)
                {
                    rb.linearVelocity = dir * force;
                }
                else
                {
                    rb.linearVelocity = Vector3.zero;
                }
                
            }
        }
    }

    private void FixedUpdate()
    {
        
    }

    private void Update()
    {
        
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
    public void AddForceToEnemyServerRpc(ulong objectID, Vector3 direction,float force ,ForceMode mode)
    {
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectID, out var enemy))
        {
            enemy.GetComponent<Rigidbody>().AddForce(direction * force, mode);
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
