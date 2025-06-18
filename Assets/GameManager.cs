using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            PlayerSpawnManager.Instance.CreatePlayerServerRpc(NetworkManager.Singleton.LocalClientId, "Jetpack");
        }
    }
}
