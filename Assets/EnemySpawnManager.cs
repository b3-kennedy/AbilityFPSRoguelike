using System.ComponentModel.Design;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawnManager : NetworkBehaviour
{
    public static EnemySpawnManager Instance;

    public GameObject enemy;


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

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;

        if (Input.GetKeyDown(KeyCode.B))
        {
            SpawnEnemyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnEnemyServerRpc()
    {
        GameObject spawnedEnemy = Instantiate(enemy);
        spawnedEnemy.GetComponent<TargetHolder>().target = NetworkManager.Singleton.LocalClient.PlayerObject.transform;
        spawnedEnemy.GetComponent<NetworkObject>().Spawn();
    }
}
