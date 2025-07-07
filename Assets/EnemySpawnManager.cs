using System.ComponentModel.Design;
using System.Threading;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnManager : NetworkBehaviour
{
    public static EnemySpawnManager Instance;

    public GameObject enemy;

    public float baseSpawnInterval;
    public float maxSpawnDistance;
    public float minSpawnDistance;
    float spawnInterval;
    float spawnTimer;

    public bool canSpawn = false;
    int enemyCount = 0;


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

    private void Start()
    {

        spawnInterval = baseSpawnInterval;
    }

    // Update is called once per frame
    void Update()
    {

        if (!IsServer) return;


        if (canSpawn && enemyCount < 10)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {

                SpawnEnemyNearPlayer();
                spawnTimer = 0;
            }
        }

    }

    void SpawnEnemyNearPlayer()
    {
        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        ulong randomNum = (ulong)Random.Range(0, playerCount);

        GameObject player = NetworkManager.Singleton.ConnectedClients[randomNum].PlayerObject.gameObject;

        Vector3 randomPos = GetRandomNavMeshPosition(player.transform.position, minSpawnDistance,maxSpawnDistance);

        if (randomPos != Vector3.zero)
        {
            SpawnEnemyServerRpc(randomPos);
        }
    }

    Vector3 GetRandomNavMeshPosition(Vector3 origin, float minDist, float maxDist)
    {
        for (int i = 0; i < 10; i++) // Try 10 times
        {
            float distance = Random.Range(minDist, maxDist);
            Vector2 randomCircle = Random.insideUnitCircle.normalized * distance;
            Vector3 randomPos = origin + new Vector3(randomCircle.x, 0, randomCircle.y);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, 5.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return Vector3.zero; // Failed to find a spot
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnEnemyServerRpc(Vector3 position)
    {
        GameObject spawnedEnemy = Instantiate(enemy, position, enemy.transform.rotation);
        spawnedEnemy.GetComponent<TargetHolder>().target = NetworkManager.Singleton.LocalClient.PlayerObject.transform;
        spawnedEnemy.GetComponent<NetworkObject>().Spawn();
        enemyCount++;
    }
}
