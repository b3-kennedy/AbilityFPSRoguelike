using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

[System.Serializable]
public class ClientPick
{
    public int clientID;
    public string characterName;
    public GameObject indicator;
}

public class PickScreenManager : NetworkBehaviour
{
    public Transform panel;
    public static PickScreenManager Instance;
    public GameObject[] selectionIcons;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var character in PlayerSpawnManager.Instance.characters)
        {
            string characterName = character.Key;
            GameObject icon = character.Value.icon;
            Instantiate(icon, panel);
            Debug.Log(characterName);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SelectCharacterServerRpc(ulong clientID, int childIndex)
    {

        
        ParentIndicatorClientRpc(childIndex, clientID);
        
    }

    [ClientRpc]
    void ParentIndicatorClientRpc(int childIndex, ulong clientID)
    {
        var pickIndex = PlayerSpawnManager.Instance.picks.FindIndex(p => (ulong)p.clientID == clientID && p.indicator != null);
        if (pickIndex != -1)
        {
            // Destroy the existing indicator
            Destroy(PlayerSpawnManager.Instance.picks[pickIndex].indicator);
        }
        GameObject indicator = Instantiate(selectionIcons[clientID]);
        Transform selectedItem = panel.GetChild(childIndex);
        ClientPick pick = new ClientPick();
        pick.characterName = selectedItem.GetComponent<SelectionBehaviour>().characterName;
        pick.indicator = indicator;
        pick.clientID = (int)clientID;
        Transform indicatorParent = selectedItem.Find("SelectedLayout");
        indicator.transform.SetParent(indicatorParent, false);
        PlayerSpawnManager.Instance.picks[(int)clientID] = pick;
        
    }

    public void LoadScene(string scene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(scene, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
