using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public Ability[] abilities;

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
    }
}
