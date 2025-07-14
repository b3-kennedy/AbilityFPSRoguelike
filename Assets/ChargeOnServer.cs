using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class ChargeOnServer : NetworkBehaviour
{

    Collider[] colliders = new Collider[20];

    [ServerRpc(RequireOwnership = false)]
    public void ChargeOnServerRpc(float pushForce)
    {
        Debug.Log("charge on server");
        int count = Physics.OverlapSphereNonAlloc(transform.position, 3f, colliders);
        for (int i = 0; i < count; i++)
        {
            Rigidbody colliderRb = colliders[i].GetComponent<Rigidbody>();
            EnemyMove enemyMove = colliders[i].GetComponent<EnemyMove>();
            Vector3 direction = (transform.position - colliders[i].transform.position).normalized;
            if (colliderRb && !enemyMove)
            {
                
                colliderRb.AddForce(-direction * pushForce, ForceMode.Impulse);
            }
            else if(colliderRb && enemyMove)
            {
                enemyMove.OnApplyForce(-direction, pushForce, ForceMode.Impulse);
                Debug.Log("apply force to enemy");
            }


        }
    }
}
