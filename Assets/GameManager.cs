using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public Ability[] abilities;
    public Dictionary<string, Upgrade> upgrades = new Dictionary<string, Upgrade>();
    public GameObject upgradePrefab;

    [Header("Upgrades")]
    public int commonChance = 60;
    public int rareChance = 35;
    public int legendaryChance = 5;

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
        abilities = Resources.LoadAll<Ability>("Abilities");
        Upgrade[] upg = Resources.LoadAll<Upgrade>("Upgrades");
        foreach (var item in upg)
        {
            if (!upgrades.ContainsKey(item.name))
            {
                upgrades.Add(item.name, item);
            }
            else
            {
                Debug.LogWarning($"Duplicate character key found: {item.name}");
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            PlayerSpawnManager.Instance.CreatePlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    public void OpenChest(Vector3 position)
    {
        int roll = Random.Range(1, 101); // 1–100 inclusive

        Upgrade.Rarity selectedRarity;

        if (roll <= commonChance)
        {
            selectedRarity = Upgrade.Rarity.COMMON;
        }
        else if (roll <= commonChance + rareChance)
        {
            selectedRarity = Upgrade.Rarity.RARE;
        }
        else
        {
            selectedRarity = Upgrade.Rarity.LEGENDARY;
        }

        Debug.Log($"Rolled {roll}, selected rarity: {selectedRarity}");

        // Filter upgrades by rarity using .Values on dictionary
        Upgrade[] matchingUpgrades = upgrades.Values.Where(u => u.rarity == selectedRarity).ToArray();

        if (matchingUpgrades.Length > 0)
        {
            Upgrade selectedUpgrade = matchingUpgrades[Random.Range(0, matchingUpgrades.Length)];
            
            ObjectSpawnManager.Instance.SpawnUpgradeServerRpc(NetworkManager.Singleton.LocalClientId, position, "UpgradePrefab", selectedUpgrade.name);

            // TODO: Assign selectedUpgrade to spawned object if needed
        }
        else
        {
            Debug.LogWarning($"No upgrades found for rarity: {selectedRarity}");
        }
    }
}
