using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public class ObjectSpawnManager : NetworkBehaviour
{
    public static ObjectSpawnManager Instance;
    public Dictionary<string, GameObject> effects = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();


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

        GameObject[] loadedObjects = Resources.LoadAll<GameObject>("Objects");
        foreach (var item in loadedObjects)
        {
            if (!objects.ContainsKey(item.name))
            {
                objects.Add(item.name, item);
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

    [ServerRpc(RequireOwnership = false)]
    public void SpawnUpgradeServerRpc(ulong clientID, Vector3 position, string objectName, string upgradeName)
    {
        if(objects.TryGetValue(objectName, out var selectedObject))
        {
            GameObject obj = Instantiate(selectedObject, position, selectedObject.transform.rotation);
            obj.GetComponent<UpgradeHolder>().effect = GameManager.Instance.upgrades[upgradeName].effect;
            obj.GetComponent<NetworkObject>().Spawn();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyObjectServerRpc(ulong objectID)
    {
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectID, out var selectedObject))
        {
            selectedObject.Despawn();
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
