using Unity.Netcode;
using UnityEngine;

public class TargetHolder : MonoBehaviour
{
    public Transform target;

    public EnemySpawnManager manager;
    public GameObject enemyPrefab;

    public void OnKilled()
    {
        if (manager != null && enemyPrefab != null)
        {
            manager.EnemyKilledServerRpc(GetComponent<NetworkObject>().NetworkObjectId);
        }
    }
}
