using UnityEngine;

public class SetSpawnFlag : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnemySpawnManager.Instance.canSpawn = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
