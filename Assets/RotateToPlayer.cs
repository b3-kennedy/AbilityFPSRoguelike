using Unity.Netcode;
using UnityEngine;

public class RotateToPlayer : MonoBehaviour
{

    Transform localPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(localPlayer == null)
        {
            if(NetworkManager.Singleton.LocalClient.PlayerObject.gameObject != null)
            {
                localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.GetComponent<PlayerData>().GetCameraGameObject().transform;
            }

        }

        Vector3 direction = localPlayer.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = lookRotation * Quaternion.Euler(0f, 180f, 0f);
    }
}
