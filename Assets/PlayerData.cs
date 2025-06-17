using Unity.Netcode;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{

    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            GetComponent<PlayerInterfaceManager>().playerInterface.SetActive(false);
        }
    }
    public bool GetOwnership()
    {
        return IsOwner;
    }

    public GameObject GetCameraGameObject()
    {
        return transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
    }

    public Transform GetGunParent()
    {
        return transform.Find("CameraHolder/Recoil/Camera/GunPosition");
    }
}
