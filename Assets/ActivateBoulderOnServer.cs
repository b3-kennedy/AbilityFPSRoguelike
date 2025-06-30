using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ActivateBoulderOnServer : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void ActivateBoulderServerRpc(string projectileID)
    {
        ActivateBoulderOnClientRpc(projectileID);
    }

    [ClientRpc]
    void ActivateBoulderOnClientRpc(string projectileID)
    {
        if(ProjectileManager.Instance.GetSpawnedProjectilesDictionary().TryGetValue(projectileID, out var boulder))
        {
            boulder.GetComponent<Boulder>().Activate();
        }
    }
}
