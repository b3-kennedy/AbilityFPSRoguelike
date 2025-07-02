using UnityEngine;

public class DamagePlayersOnCollision : MonoBehaviour
{

    public float damage = 5f;

    private void Start()
    {
        
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.GetComponent<PlayerMovement>())
        {
            other.transform.GetComponent<Health>().TakeDamageServerRpc(damage);
        }
    }
}
