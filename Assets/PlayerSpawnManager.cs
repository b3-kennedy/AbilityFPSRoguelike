using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawnManager : NetworkBehaviour
{

    public static PlayerSpawnManager Instance { get; private set; }
    public GameObject defaultPlayerPrefab;
    public Dictionary<string, CharacterData> characters = new Dictionary<string, CharacterData>();
    public List<ClientPick> picks = new List<ClientPick>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
        LoadCharacterData();
    }

    private void LoadCharacterData()
    {
        CharacterData[] allCharacters = Resources.LoadAll<CharacterData>("CharacterData");

        foreach (CharacterData character in allCharacters)
        {
            if (!characters.ContainsKey(character.name))
            {
                characters.Add(character.name, character);
                Debug.Log($"Loaded {character.name}");
            }
            else
            {
                Debug.LogWarning($"Duplicate character key found: {character.name}");
            }
        }
    }

    private void Start()
    {
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreatePlayerServerRpc(ulong clientID)
    {
        Debug.Log("Create Player");
        string characterName = picks[(int)clientID].characterName;
        NetworkObject spawnedPlayer = Instantiate(characters[characterName].prefab).GetComponent<NetworkObject>();
        spawnedPlayer.name = spawnedPlayer.name + "_Player"+clientID;
        spawnedPlayer.SpawnAsPlayerObject(clientID, false);
    }
}
