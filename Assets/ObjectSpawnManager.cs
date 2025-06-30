using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public class ObjectSpawnManager : NetworkBehaviour
{
    public static ObjectSpawnManager Instance;
    public Dictionary<string, GameObject> effects = new Dictionary<string, GameObject>();


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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject[] loadedEffects = Resources.LoadAll<GameObject>("Effects");
        foreach (var item in loadedEffects)
        {
            if (!effects.ContainsKey(item.name))
            {
                effects.Add(item.name, item);
            }
            else
            {
                Debug.LogWarning($"Duplicate character key found: {item.name}");
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnEffectServerRpc(ulong clientID, Vector3 position, string effectName)
    {
        SpawnEffectClientRpc(clientID, position, effectName);
    }

    [ClientRpc]
    void SpawnEffectClientRpc(ulong clientID, Vector3 position, string effectName)
    {
        if (NetworkManager.Singleton.LocalClientId == clientID) return;

        if(effects.TryGetValue(effectName, out var effect))
        {
            Instantiate(effect, position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
