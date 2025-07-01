using UnityEngine;

[System.Serializable]
public abstract class UpgradeEffect : ScriptableObject
{
    GameObject player;
    public int count;

    public void SetPlayer(GameObject p)
    {
        player = p;
    }

    public GameObject GetPlayer()
    {
        return player;
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
