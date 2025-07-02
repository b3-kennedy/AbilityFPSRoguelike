using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

public class UpgradeAssetGenerator
{
    [MenuItem("Tools/Generate All Upgrade Assets")]
    public static void GenerateUpgradeAssets()
    {
        string upgradesPath = "Assets/Resources/UpgradeEffects/";

        // Ensure directory exists
        if (!Directory.Exists(upgradesPath))
        {
            Directory.CreateDirectory(upgradesPath);
            AssetDatabase.Refresh();
        }

        // Find all Ability types
        var upgradeTypes = typeof(UpgradeEffect).Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(UpgradeEffect)) && !t.IsAbstract);

        foreach (var type in upgradeTypes)
        {
            string assetPath = upgradesPath + type.Name + ".asset";

            // Check if asset already exists
            if (File.Exists(assetPath))
            {
                continue;
            }

            // Create the asset
            UpgradeEffect upgradeAsset = ScriptableObject.CreateInstance(type) as UpgradeEffect;
            AssetDatabase.CreateAsset(upgradeAsset, assetPath);
            Debug.Log("Created Upgrade Asset: " + assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}