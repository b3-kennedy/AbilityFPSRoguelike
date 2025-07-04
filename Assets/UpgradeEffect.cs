using UnityEngine;
using TMPro;
using Unity.Netcode;

[System.Serializable]
public abstract class UpgradeEffect : ScriptableObject
{
    GameObject player;
    public int count;
    public string title;
    GameObject upgradeObject;
    public void SetPlayer(GameObject p)
    {
        player = p;
    }

    public void SetUpgradeObject(GameObject obj)
    {
        upgradeObject = obj;
    }

    public GameObject GetUpgradeObject()
    {
        return upgradeObject;
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public virtual void OnSpawned()
    {
        SetPlayer(NetworkManager.Singleton.LocalClient.PlayerObject.gameObject);
        SetToolTipTitle(title);
    }

    public GameObject GetToolTip()
    {
        
        return GetPlayer().transform.Find("PlayerInterface/ToolTipPanel").gameObject;
    }

    public TextMeshProUGUI GetToolTipTitle()
    {
        return GetPlayer().transform.Find("PlayerInterface/ToolTipPanel/Title").GetComponent<TextMeshProUGUI>();
    }

    public TextMeshProUGUI GetToolTipDescription()
    {
        return GetPlayer().transform.Find("PlayerInterface/ToolTipPanel/Description").GetComponent<TextMeshProUGUI>();
    }

    public void SetToolTipTitle(string title)
    {
        GetToolTipTitle().text = title;
    }

    public void SetToolTipDescription(string desc)
    {
        GetToolTipDescription().text = desc;
    }

    public Gun GetGun()
    {
        if (player)
        {
            return player.transform.Find("CameraHolder/Recoil/Camera/GunPosition/GunParent/Gun").GetComponent<Gun>();
        }
        Debug.Log("Cannot get gun, caster is null");
        return null;
    }

    public abstract void Apply();

    public abstract void Remove();
}
