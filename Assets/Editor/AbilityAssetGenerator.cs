using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

public class AbilityAssetGenerator
{
    [MenuItem("Tools/Generate All Ability Assets")]
    public static void GenerateAbilityAssets()
    {
        string abilitiesPath = "Assets/Resources/Abilities/";

        // Ensure directory exists
        if (!Directory.Exists(abilitiesPath))
        {
            Directory.CreateDirectory(abilitiesPath);
            AssetDatabase.Refresh();
        }

        // Find all Ability types
        var abilityTypes = typeof(Ability).Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(Ability)) && !t.IsAbstract);

        foreach (var type in abilityTypes)
        {
            string assetPath = abilitiesPath + type.Name + ".asset";

            // Check if asset already exists
            if (File.Exists(assetPath))
            {
                continue;
            }

            // Create the asset
            Ability abilityAsset = ScriptableObject.CreateInstance(type) as Ability;
            AssetDatabase.CreateAsset(abilityAsset, assetPath);
            Debug.Log("Created Ability Asset: " + assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}