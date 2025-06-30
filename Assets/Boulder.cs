using UnityEngine;

public class Boulder : MonoBehaviour
{

    Rigidbody rb;
    float damage;
    float radius;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    public void Activate()
    {
        rb.isKinematic = false;
        rb.AddForce(-transform.up * 10, ForceMode.Impulse);
    }

    public void SetValues(float dmg, float rad)
    {
        damage = dmg;
        radius = rad;
    }

    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
        Collider[] colliders = new Collider[50];
        int count = Physics.OverlapSphereNonAlloc(transform.position, radius, colliders);
        for (int i = 0; i < count; i++)
        {
            Health health = colliders[i].GetComponent<Health>();
            UnitData data = colliders[i].GetComponent<UnitData>();
            if(health && data && data.GetTeam() == UnitData.Team.BAD)
            {
                health.TakeDamageServerRpc(damage);
            }
        }
        ProjectileManager.Instance.DestroyLocalProjectileFromServerRpc(GetComponent<Projectile>().ID);
    }
}
