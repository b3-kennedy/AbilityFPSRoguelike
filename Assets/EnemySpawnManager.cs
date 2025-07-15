using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

[System.Serializable]
public class EnemySpawnChance
{
    public GameObject enemy;
    public float chance;
    public int maxInGame;
}

[System.Serializable]
public class EnemySpawn
{
    public List<EnemySpawnChance> enemies;
}


public class EnemySpawnManager : NetworkBehaviour
{
    public static EnemySpawnManager Instance;

    public GameObject enemy;

    public float baseSpawnInterval;
    public float maxSpawnDistance;
    public float minSpawnDistance;
    float spawnInterval;
    float spawnTimer;
    public int difficultyLevel;

    public bool canSpawn = false;
    int enemyCount = 0;
    float gameTimer;
    public float difficultyIncreaseInterval;
    int lastLoggedTime;

    public List<EnemySpawn> enemySpawnChances;

    public Dictionary<GameObject, int> spawnedEnemyCounts = new Dictionary<GameObject, int>();




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
        GameObject[] enemies = Resources.LoadAll<GameObject>("Enemies");
        Debug.Log(enemies.Length);
        foreach (GameObject enemy in enemies)
        {
            if (!spawnedEnemyCounts.ContainsKey(enemy))
            {
                spawnedEnemyCounts.Add(enemy, 0);
            }
            else
            {
                Debug.LogWarning($"Duplicate character key found: {enemy.name}");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!IsServer) return;


        if (canSpawn && enemyCount < 100)
        {
            gameTimer += Time.deltaTime;
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {

                SpawnEnemyNearPlayer();
                spawnTimer = 0;
            }



            int currentTime = Mathf.FloorToInt(gameTimer / difficultyIncreaseInterval);
            if (currentTime > lastLoggedTime)
            {
                lastLoggedTime = currentTime;
                Debug.Log("Increase difficulty");
                difficultyLevel++;
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
    public void EnemyKilledServerRpc(ulong networkID)
    {
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkID, out var enemy))
        {
            TargetHolder holder = enemy.GetComponent<TargetHolder>();
            if(spawnedEnemyCounts.TryGetValue(holder.enemyPrefab, out var value))
            {
                spawnedEnemyCounts[holder.enemyPrefab]--;
                Debug.Log("reduced count");
            }
        }
        else
        {
            Debug.Log("Not reduced enemy probably destroyed before this gets called");
        }
    }


    [ServerRpc(RequireOwnership = false)]
    void SpawnEnemyServerRpc(Vector3 position, ulong playerIndex)
    {
        var validEnemies = enemySpawnChances[difficultyLevel].enemies
            .Where(e => spawnedEnemyCounts[e.enemy] < e.maxInGame)
            .ToList();

        if (validEnemies.Count == 0)
        {
            Debug.Log("No enemies available to spawn (all reached maxInGame)");
            return;
        }

        float totalChance = validEnemies.Sum(e => e.chance);
        float roll = Random.Range(0f, totalChance);

        float cumulativeChance = 0f;
        foreach (var entry in validEnemies)
        {
            cumulativeChance += entry.chance;
            if (roll <= cumulativeChance)
            {
                Debug.Log("Spawned: " + entry.enemy.name);
                GameObject spawnedEnemy = Instantiate(entry.enemy, position, enemy.transform.rotation);
                spawnedEnemyCounts[entry.enemy]++;

                TargetHolder holder = spawnedEnemy.GetComponent<TargetHolder>();
                holder.target = NetworkManager.Singleton.ConnectedClients[playerIndex].PlayerObject.transform;
                holder.manager = this;
                holder.enemyPrefab = entry.enemy;
                spawnedEnemy.GetComponent<NetworkObject>().Spawn();

                enemyCount++;
                break;
            }
        }

    }
}
