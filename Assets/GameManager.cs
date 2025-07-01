using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public Ability[] abilities;

    public GameObject upgradePrefab;

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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            PlayerSpawnManager.Instance.CreatePlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ObjectSpawnManager.Instance.SpawnObjectServerRpc(NetworkManager.Singleton.LocalClientId, Vector3.zero, "UpgradePrefab");
        }
    }
}
