using UnityEngine;

public class DamagePlayersOnCollision : MonoBehaviour
{

    private void Start()
    {
        
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.GetComponent<PlayerMovement>())
        {
            other.transform.GetComponent<Health>().TakeDamageServerRpc(5f);
        }
    }
}
