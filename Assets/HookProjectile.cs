using Unity.Netcode;
using UnityEngine;

public class HookProjectile : NetworkBehaviour
{
    public GameObject player;
    public Transform hookPoint;
    Rigidbody hookedRb;
    bool hooked;

    PlayerData playerData;

    private void Start()
    {
    }
    private void OnCollisionEnter(Collision other)
    {
        UnitData data = other.collider.gameObject.GetComponent<UnitData>();
        if(data && data.GetWeightClass() == UnitData.WeightClass.LIGHT)
        {
            
            hookedRb = other.collider.GetComponent<Rigidbody>();
        }
        else
        {
            hookedRb = player.GetComponent<Rigidbody>();
            GameObject point = new GameObject();
            point.transform.position = transform.position;
            hookPoint = point.transform;
            Debug.Log("PLAYER TO OBJECT");
        }
        hooked = true;
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void Update()
    {
        if (player == null || hookPoint == null)
        {
            GetComponent<Collider>().enabled = false;
            return;
        };

        if (hooked)
        {
            
            if(Vector3.Distance(hookedRb.transform.position, hookPoint.position) >= 0.5f)
            {
                Vector3 dir = (hookPoint.position - hookedRb.transform.position).normalized;
                ProjectileManager.Instance.MoveObjectOnServerRpc(hookedRb.gameObject.GetComponent<NetworkObject>().NetworkObjectId, dir, 25f, hookPoint.position);
            }
            else
            {
                hooked = false;
                hookedRb.linearVelocity = Vector3.zero;
                Destroy(gameObject);
            }

        }
    }
}
