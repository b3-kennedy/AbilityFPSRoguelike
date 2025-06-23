using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class VacuumProjectile : MonoBehaviour
{
    Collider[] colliders = new Collider[50];
    public float explosionRadius;
    public float explosionForce;
    GameObject playerWhoShot;
    ulong shooterID;
    public int altValue = 1;
    [HideInInspector] public UnityEvent onCollide;


    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.gameObject == playerWhoShot || other.collider.GetComponent<VacuumProjectile>() || other.collider.GetComponent<Gun>()) return;

        int count = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, colliders);
        for (int i = 0; i < count; i++)
        {
            Rigidbody rb = colliders[i].attachedRigidbody;
            Vector3 dir = (colliders[i].transform.position - transform.position).normalized;
            if (rb)
            {
                rb.AddForce(dir * explosionForce * altValue, ForceMode.Impulse);
            }
        }
        ProjectileManager.Instance.CreateExplosionServerRpc(NetworkManager.Singleton.LocalClientId, transform.position, explosionRadius, explosionForce * altValue);
        onCollide.Invoke();
        Destroy(gameObject);
        

    }
}
