using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ServerKnifeHolder : NetworkBehaviour
{

    public List<GameObject> knives = new List<GameObject>();

    [ServerRpc(RequireOwnership = false)]
    public void SpawnKnifeServerRpc(ulong clientId, string projectileName, Vector3 position, Quaternion rotation, ulong parentID)
    {
        if (ProjectileManager.Instance.GetProjectilesDictionary().TryGetValue(projectileName, out var projectile))
        {
            GameObject spawnedProjectile = Instantiate(projectile, position, rotation);
            spawnedProjectile.transform.position = position;
            spawnedProjectile.transform.rotation = rotation;
            spawnedProjectile.GetComponent<Rigidbody>().isKinematic = true;

            spawnedProjectile.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            spawnedProjectile.GetComponent<ExplosiveThrowingKnifeProjectile>().enabled = false;
            ParentKnifeClientRpc(parentID, spawnedProjectile.GetComponent<NetworkObject>().NetworkObjectId);

            Debug.Log("called");

        }
    }

    [ClientRpc]
    void ParentKnifeClientRpc(ulong parentID, ulong knifeID)
    {
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(knifeID, out var knife) 
            && NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(parentID, out var parent))
        {
            knife.GetComponent<ExplosiveThrowingKnifeProjectile>().enabled = false;
            if (IsServer)
            {
                knife.transform.SetParent(parent.transform);
            }
            knives.Add(knife.gameObject);
        }
    }

    [ClientRpc]
    void AddKnifeClientRpc(ulong objectID)
    {
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectID, out var knife))
        {
            knives.Add(knife.gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyKnivesServerRpc(float damage)
    {
        DestroyKnivesClientRpc(damage);
    }

    [ClientRpc]
    void DestroyKnivesClientRpc(float damage)
    {
        for (int i = 0; i < knives.Count; i++)
        {
            if (knives[i].transform.parent)
            {
                Health heal = knives[i].transform.parent.GetComponent<Health>();
                UnitData data = knives[i].transform.parent.GetComponent<UnitData>();
                if (heal && data.GetTeam() == UnitData.Team.BAD)
                {
                    heal.TakeDamageServerRpc(damage);
                }
            }
        }

        for (int i = 0; i < knives.Count; i++)
        {
            Destroy(knives[i]);
        }
        knives.Clear();
        knives.Clear();
    }

}
