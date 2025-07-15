using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(EnemySpawnManager))]
public class EnemySpawnManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EnemySpawnManager manager = (EnemySpawnManager)target;

        if (GUILayout.Button("Auto-Fill Enemies from Resources/Enemies"))
        {
            AutoFillEnemies(manager);
        }
    }

    private void AutoFillEnemies(EnemySpawnManager manager)
    {
        GameObject[] allEnemies = Resources.LoadAll<GameObject>("Enemies");

        if (allEnemies.Length == 0)
        {
            Debug.LogWarning("No prefabs found in Resources/Enemies.");
            return;
        }

        if (manager.enemySpawnChances == null)
            manager.enemySpawnChances = new List<EnemySpawn>();

        foreach (var spawnGroup in manager.enemySpawnChances)
        {
            if (spawnGroup.enemies == null)
                spawnGroup.enemies = new List<EnemySpawnChance>();

            List<EnemySpawnChance> groupEnemies = spawnGroup.enemies;

            foreach (var enemy in allEnemies)
            {
                bool exists = groupEnemies.Exists(e => e.enemy == enemy);

                if (!exists)
                {
                    groupEnemies.Add(new EnemySpawnChance
                    {
                        enemy = enemy,
                        chance = 1f,
                        maxInGame = 5
                    });

                    Debug.Log($"Added {enemy.name} to spawn group");
                }
            }
        }

        foreach (var spawnGroup in manager.enemySpawnChances)
        {
            if (spawnGroup.enemies == null)
                continue;

            int before = spawnGroup.enemies.Count;
            spawnGroup.enemies.RemoveAll(e => e == null || e.enemy == null);
            int after = spawnGroup.enemies.Count;

            if (before != after)
                Debug.Log($"Removed {before - after} null entries from a spawn group");
        }

        EditorUtility.SetDirty(manager);
    }
}
