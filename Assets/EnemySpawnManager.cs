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


        if (canSpawn && enemyCount < 100)
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
            SpawnEnemyServerRpc(randomPos, randomNum);
        }
    }

    Vector3 GetRandomNavMeshPosition(Vector3 origin, float minDist, float maxDist)
    {
        for (int i = 0; i < 10; i++) // Try up to 10 times
        {
            float distance = Random.Range(minDist, maxDist);
            Vector2 randomCircle = Random.insideUnitCircle.normalized * distance;
            Vector3 candidatePos = origin + new Vector3(randomCircle.x, 50f, randomCircle.y); // Start ray 10 units up
            Debug.DrawRay(candidatePos, Vector3.down * 90f, Color.red, 1f);
            // Cast ray down from above the candidate position
            if (Physics.Raycast(candidatePos, Vector3.down, out RaycastHit rayHit, 90f, LayerMask.GetMask("Ground"))) // Use your ground/floor layer
            {
                Vector3 groundPos = rayHit.point;

                // Check if it's a valid NavMesh position near where the ray hit
                if (NavMesh.SamplePosition(groundPos, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
                {
                    return navHit.position;
                }
            }
        }

        return Vector3.zero; // Failed to find a spot
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnEnemyServerRpc(Vector3 position, ulong playerIndex)
    {
        GameObject spawnedEnemy = Instantiate(enemy, position, enemy.transform.rotation);
        spawnedEnemy.GetComponent<TargetHolder>().target = NetworkManager.Singleton.ConnectedClients[playerIndex].PlayerObject.transform;
        spawnedEnemy.GetComponent<NetworkObject>().Spawn();
        enemyCount++;
    }
}
