using UnityEngine;
using UnityEngine.AI;

public class ChestSpawnManager : MonoBehaviour
{
    public GameObject chestPrefab;
    public Bounds spawnArea;
    public LayerMask groundMask;
    public float height;
    public int chestCount;
    public float overlapRadius = 1f;
    public int maxAttempts = 1000; // to avoid infinite loops

    private int spawnedChestCount = 0;

    private void Start()
    {
        int attempts = 0;

        while (spawnedChestCount < chestCount && attempts < maxAttempts)
        {
            if (TrySpawnChest())
            {
                spawnedChestCount++;
            }

            attempts++;
        }

        if (spawnedChestCount < chestCount)
        {
            Debug.LogWarning($"Only spawned {spawnedChestCount}/{chestCount} chests after {attempts} attempts.");
        }
    }

    bool TrySpawnChest()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(spawnArea.min.x, spawnArea.max.x),
            spawnArea.center.y + height,
            Random.Range(spawnArea.min.z, spawnArea.max.z)
        );

        Debug.Log(randomPosition);

        if (Physics.Raycast(randomPosition, Vector3.down, out RaycastHit hit, 1000f, groundMask))
        {
            Quaternion baseRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Quaternion randomTwist = Quaternion.AngleAxis(Random.Range(0f, 360f), hit.normal);
            Quaternion finalRotation = randomTwist * baseRotation;
            Instantiate(chestPrefab, new Vector3(hit.point.x, hit.point.y + 0.35f, hit.point.z), finalRotation);
            return true;
        }

        return false;
    }

}
